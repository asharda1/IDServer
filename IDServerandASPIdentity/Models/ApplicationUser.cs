using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IDServer.Models
{
  // Add profile data for application users by adding properties to the ApplicationUser class
  //Added all extra properties required for the user
  public class ApplicationUser : IdentityUser
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid TenantId { get; set; }
    public bool IsMultiTenant { get; set; }
    public int Role { get; set; }
    public bool IsActive { get; set; }
  }
}
