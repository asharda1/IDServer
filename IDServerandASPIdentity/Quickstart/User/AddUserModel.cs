using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDServer.Models
{
  public class AddUserModel
  {

    [Required]
    [MaxLength(200)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(200)]
    public string LastName { get; set; }


    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public int Role { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
