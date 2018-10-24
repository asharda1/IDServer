using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDServer.Model
{
  public class RegisterViewModel
  {

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "First Name")]
    [StringLength(200)]
    public string FirstName { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name")]
    [StringLength(200)]
    public string LastName { get; set; }


    [Required]
    [Display(Name = "TenantId")]
    public Guid TenantId { get; set; }

    [Required]
    [Display(Name = "IsMultiTenant")]
    public bool IsMultiTenant { get; set; }

    [Required]
    [Display(Name = "Role")]
    public int Role { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
  }
}
