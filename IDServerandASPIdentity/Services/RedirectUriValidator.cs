
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDServer.Services
{
  public class RedirectUriValidator : IRedirectUriValidator
  {

    public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
    {
      var reqHost = new Uri(requestedUri).Host;
      var p = reqHost.LastIndexOf(".");
      var reqDomain = reqHost.Substring(p + 1);
      bool hasRedirectDomain = false;
      foreach (string cUri in client.RedirectUris)
      {
        var uri = new Uri(cUri);
        var host = uri.Host;
        p = host.LastIndexOf(".");
        var domain = host.Substring(p + 1);
        if (reqDomain == domain) { hasRedirectDomain = true; break; }
      }
      return Task.FromResult(hasRedirectDomain);

    }

    public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
    {
      var reqHost = new Uri(requestedUri).Host;
      var p = reqHost.LastIndexOf(".");
      var reqDomain = reqHost.Substring(p + 1);
      bool hasPostRedirectDomain = false;
      foreach (string cUri in client.PostLogoutRedirectUris)
      {
        var uri = new Uri(cUri);
        var host = uri.Host;
        p = host.LastIndexOf(".");
        var domain = host.Substring(p + 1);
        if (reqDomain == domain) { hasPostRedirectDomain = true; break; }
      }
      return Task.FromResult(hasPostRedirectDomain);
    }
  }
}