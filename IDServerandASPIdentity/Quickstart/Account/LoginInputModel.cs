﻿



using System.ComponentModel.DataAnnotations;

namespace IDServer.Models
{
  public class LoginInputModel
  {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberLogin { get; set; }
    public string ReturnUrl { get; set; }
    public string ClientId { get; set; }
    public string TenantId
    {
      get; set;
    }
  }
}