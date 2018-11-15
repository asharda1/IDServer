



using System;
using System.Collections.Generic;
using System.Linq;

namespace IDServer.Models {
  public class LoginViewModel : LoginInputModel {
    public bool AllowRememberLogin
    {
      get; set;
    }
    public bool EnableLocalLogin
    {
      get; set;
    }
    //* to customize the login page for branding - Ewapps
    public string ClientId
    {
      get; set;
    }
    public ClientTenantModel ClientTenantModel
    {
      get; set;
    }
    public IEnumerable<ExternalProvider> ExternalProviders
    {
      get; set;
    }
    public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

    public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
    public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
  }
}