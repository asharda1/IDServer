using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDServer.Models
{
  public class UserChangePasswordModel
  {

    [Required]
    [EmailAddress]
    public string Email
    {
      get; set;
    }

    [Required]
    public string OldPassword
    {
      get; set;
    }

    [Required]
    public string NewPassword
    {
      get; set;
    }

    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword
    {
      get; set;
    }


  }
}
