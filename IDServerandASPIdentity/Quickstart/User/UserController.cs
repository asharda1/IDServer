using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IDServer.Models;
using IDServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDServer.Controllers
{
  /// <summary>
  /// User controller is the single place for all custome API methods not provided by Identity server
  /// </summary>
  public class UserController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger _logger;

    public UserController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           IEmailSender emailSender,
           ILogger<UserController> logger) {
      _userManager = userManager;
      _signInManager = signInManager;
      _emailSender = emailSender;
      _logger = logger;
    }

    /// <summary>
    /// Add user by API call to this method. User should be authorized to add new users
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>


    [HttpPost]
    [Produces("application/json")]
    public async Task<IActionResult> Add([FromBody] AddUserModel model) {
      if (ModelState.IsValid)
      {
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, TenantId = model.TenantId, IsMultiTenant = false, Role = model.Role, IsActive = model.IsActive };

        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
          _logger.LogInformation("User created a new account.");
          //ASha SHarda - Commented id code as now login flow will be by email code
          // var id = user.Id;
          // return Ok(id);
          //Asha Sharda - Uncommented code for email code flow.
          var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          ////  var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
          //// await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
          AddUserResponseModel UserResponse = new AddUserResponseModel();
          UserResponse.Code = HttpUtility.UrlEncode(code); //Do URL encoding as this will be part of URL
          UserResponse.UserId = user.Id;
          return Ok(UserResponse);
        }
        else
        {
          _logger.LogError("User not created.");
          return StatusCode(500);
        }
      }
      else
      {
        _logger.LogError("Wrong user object");
        return BadRequest();
      }
    }

    /// <summary>
    /// UI to set the password
    /// </summary>
    /// <param name="code"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult SetPassword(string code = null, string returnUrl = null) {
      var model = new SetUserPasswordModel { };
      model.Code = code ?? throw new ApplicationException("A code must be supplied for set password operation.");
      model.RedirectUrl = returnUrl;
      return View(model);
    }

    /// <summary>
    /// Set password operation, it checks for the code 
    /// sent and matches the user for that code and then  the password is set.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="code"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetUserPasswordModel model) {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        // return StatusCode(500);
        ModelState.AddModelError("", "Email address does not exists.");
        return View(model);
      }
      if (!string.IsNullOrEmpty(user.PasswordHash)) {
        ModelState.AddModelError("", "Password already set for this email address.");
        return View(model);
      }
 
      var result = await _userManager.ConfirmEmailAsync(user, model.Code);
      //Check for the supplied code, it should match with the code generated for user
      if (result.Errors != null && result.Errors.Count() > 0)
      {
        _logger.LogError("Unauthorized access  to the set password:" + user.Email);
        //return StatusCode(401);//Unauthorized
        ModelState.AddModelError("", "Unauthorized access  to the set password.");
        return View(model);
      }

      result = await _userManager.AddPasswordAsync(user, model.NewPassword);
      if (result.Succeeded)
      {
        _logger.LogInformation("Password is set for:" + user.Email);
        return Redirect(model.RedirectUrl);
      }
      _logger.LogError("Error in adding password");
      //return StatusCode(500);
      ModelState.AddModelError("", "Error in adding password");
      return View(model);

    }


    /// <summary>
    /// Set password operation, it checks for the code 
    /// sent and matches the user for that code and then  the password is set.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Produces("application/json")]
    public async Task<IActionResult> Password([FromBody] UserPasswordModel model) {
      if (!ModelState.IsValid)
      {
        return StatusCode(422);
      }
      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null || !string.IsNullOrEmpty(user.PasswordHash))
      {
        // Don't reveal that the user does not exist
        return StatusCode(422);
      }
      var result = await _userManager.AddPasswordAsync(user, model.Password);
      if (result.Succeeded)
      {
        _logger.LogInformation("Password is set for:" + user.Email);
        return Ok();
      }
      _logger.LogError("Error in adding password");
      return StatusCode(500);
    }

    /// <summary>
    /// Set password operation, it checks for the code 
    /// sent and matches the user for that code and then  the password is set.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Produces("application/json")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordModel model) {
      if (!ModelState.IsValid)
      {
        return StatusCode(422);
      }
      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return StatusCode(422);
      }

      var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
      if (result.Succeeded)
      {
        _logger.LogInformation("Password is updated for:" + user.Email);
        return Ok();
      }
      _logger.LogError("Error in updating  password");
      return StatusCode(500);
    }

    [HttpGet]
    [AllowAnonymous]
    // GET: /AccountAdmin/ResetPassword
    public ActionResult ResetPassword([FromQuery] string code, [FromQuery] string userId, [FromQuery] string returnUrl) {
      if (code == null)
      {
        return StatusCode(400);//Bad request
      }
      ResetPasswordViewModel model = new ResetPasswordViewModel() { Code = code, UserId = userId, ReturnUrl = returnUrl };
      return View(model);
    }

    //
    // POST: /AccountAdmin/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var user = await _userManager.FindByIdAsync(model.UserId);
      if (user == null)
        return StatusCode(400);
      ViewData["TenantId"] = user.TenantId;
      ViewData["ReturnUrl"] = model.ReturnUrl;
      var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);

      if (result.Succeeded)
        return View("ResetPasswordConfirmation");
      return StatusCode(400);
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult ForgotPassword([FromQuery] string tenantId = null, [FromQuery] string returnUrl = null) {
      ForgotPasswordViewModel model = new ForgotPasswordViewModel() { TenantId = tenantId, ReturnUrl = returnUrl };
      return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) {
      if (ModelState.IsValid)
      {
        ViewData["TenantId"] = model.TenantId;
        ViewData["ReturnUrl"] = model.ReturnUrl;
        var user = await _userManager.FindByNameAsync(model.Email);
        if (user == null)
        {
          // Don't reveal that the user does not exist or is not confirmed
          return View("ForgotPasswordConfirmation");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        /*   var callbackUrl = Url.Action("ResetPassword", "Account",
       new
       {
         UserId = user.Id,
         TenantId = user.TenantId,
         code = code
       }, protocol: Request.Url.Scheme);
           await _userManager.SendEmailAsync(user.Id, "Reset Password",
       "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");*/

        return View("ForgotPasswordConfirmation");
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }



    /// <summary>
    /// Delete User
    /// 
    /// </summary>
    /// <param userid="">Identityserver userIdd</param>
    /// <returns></returns>
    [HttpDelete]
    [AllowAnonymous]

    public async Task<IActionResult> Delete(string userid) {
      if (string.IsNullOrEmpty(userid))
      {
        return StatusCode(422);
      }
      var user = await _userManager.FindByIdAsync(userid);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return Ok();
      }
      var result = await _userManager.DeleteAsync(user);
      if (result.Succeeded)
      {
        _logger.LogInformation("User is deleted" + user.Email);
        return Ok();
      }
      _logger.LogError("Error in deleting user");
      return StatusCode(500);
    }

    /// <summary>
    /// Set Active User
    /// 
    /// </summary>
    /// <param userid="">Identityserver userIdd</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    //[Authorize(AuthenticationSchemes = "token")]
    public async Task<IActionResult> MarkUserActive(string userid) {
      if (string.IsNullOrEmpty(userid))
      {
        return StatusCode(422);
      }
      var user = await _userManager.FindByIdAsync(userid);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return Ok();
      }
      user.IsActive = true;
      var result = await _userManager.UpdateAsync(user);
      if (result.Succeeded)
      {
        _logger.LogInformation("User is set Active" + user.Email);
        return Ok();
      }
      _logger.LogError("Error in setting  user properties");
      return StatusCode(500);
    }
    /// <summary>
    /// Set Active User
    /// 
    /// </summary>
    /// <param userid="">Identityserver userIdd</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> MarkUserInActive(string userid) {
      if (string.IsNullOrEmpty(userid))
      {
        return StatusCode(422);
      }
      var user = await _userManager.FindByIdAsync(userid);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return Ok();
      }
      user.IsActive = false;
      var result = await _userManager.UpdateAsync(user);
      if (result.Succeeded)
      {
        _logger.LogInformation("User is set In active" + user.Email);
        return Ok();
      }
      _logger.LogError("Error in setting  user properties");
      return StatusCode(500);
    }
  }
}