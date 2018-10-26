  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Linq;
  using System.Threading.Tasks;

  namespace IDServer.Models
  {
    public class ForgotPasswordViewModel
    {
      [Required]
      [EmailAddress]
      public string Email
      {
        get; set;
      }
    [Required]
    
    public string TenantId
    {
      get; set;
    }
    public string ReturnUrl
    {
      get; set;
    }
  }
  }

