﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDServer.Models
{
  public class UserPasswordModel
  {

    [Required]
    [EmailAddress]
    public string Email
    {
      get; set;
    }

    [Required]
    public string Password
    {
      get; set;
    }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword
    {
      get; set;
    }


  }
}
