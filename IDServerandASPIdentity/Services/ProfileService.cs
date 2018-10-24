using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IDServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IDServer.Services
{
  public class ProfileService : IProfileService
  {
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) {
      _userManager = userManager;
      _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context) {
      var sub = context.Subject.GetSubjectId();
      ApplicationUser user = await _userManager.FindByIdAsync(sub);
      var principal = await _claimsFactory.CreateAsync(user);

      var claims = principal.Claims.ToList();
      claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
      claims.Add(new Claim("firstname", user.FirstName));


      claims.Add(new Claim("firstname", user.FirstName));
      claims.Add(new Claim("lastname", user.LastName));
      claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
      claims.Add(new Claim("uid", user.Id));
      claims.Add(new Claim("tenantid", user.TenantId.ToString()));
      claims.Add(new Claim("ismultitenant", user.IsMultiTenant.ToString()));
      claims.Add(new Claim("role", user.Role.ToString()));
      claims.Add(new Claim(JwtClaimTypes.Scope, ("platformprop")));
      context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context) {
      var sub = context.Subject.GetSubjectId();
      var user = await _userManager.FindByIdAsync(sub);
      context.IsActive = user != null;
    }
  }
}
