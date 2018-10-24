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
           ILogger<UserController> logger)
    {
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
    public async Task<IActionResult> Add([FromBody] AddUserModel model)
    {
      if (ModelState.IsValid)
      {
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, TenantId = model.TenantId, IsMultiTenant = false, Role = model.Role };
        
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
          _logger.LogInformation("User created a new account.");
          var id = user.Id;
          //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          ////  var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
          //// await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
          //code = HttpUtility.UrlEncode(code); //Do URL encoding as this will be part of URL
          return Ok(id);
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
    public IActionResult SetPassword(string code = null, string returnUrl = null)
    {
      /*   if (code == null)
       {
         throw new ApplicationException("A code must be supplied for set password operation.");
       }*/
      ViewData["ReturnUrl"] = returnUrl;
      ViewData["code"] = code;
      var model = new SetUserPasswordModel { };
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
    public async Task<IActionResult> SetPassword([FromBody]SetUserPasswordModel model, [FromQuery]string code = null, [FromQuery] string returnUrl = null)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null || !string.IsNullOrEmpty(user.PasswordHash))
      {
        // Don't reveal that the user does not exist
        return StatusCode(500);
      }

      var result = await _userManager.ConfirmEmailAsync(user, code);
      //Check for the supplied code, it should match with the code generated for user
      if (result.Errors != null && result.Errors.Count()>0)
      {
        _logger.LogError("Unauthorized access to to the set password:" + user.Email);
        return StatusCode(401);//Unauthorized
      }

      result = await _userManager.AddPasswordAsync(user, model.NewPassword);
      if (result.Succeeded)
      {
        _logger.LogInformation("Password is set for:" + user.Email);
        return Ok();
      }
      _logger.LogError("Error in adding password");
      return StatusCode(500);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
      return View();
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

  }
}