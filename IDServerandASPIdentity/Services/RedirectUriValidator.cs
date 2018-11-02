/* Copyright © 2018 eWorkplace Apps (https://www.eworkplaceapps.com/). All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * 
 * Author: ASha sharda
 * Date: 29 October 2018
 * 
 * Contributor/s: 
 * Last Updated On: 
 */
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDServer.Services
{
/// <summary>
/// Override interface that is responsible to validate the Return URLs of the client
/// </summary>
  public class RedirectUriValidator : IRedirectUriValidator
  {

/// <summary>
/// Method executes to check the return URL of client after sign-In or silent signin
/// </summary>
/// <param name="requestedUri"></param>
/// <param name="client"></param>
/// <returns></returns>
    public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
    {
      var reqHost = new Uri(requestedUri).Host;
      var reqPort = new Uri(requestedUri).Port;
      bool hasRedirectDomain = false;
      foreach (string cUri in client.RedirectUris)
      {
        var uri = new Uri(cUri);
        var host = uri.Host;
        var port = uri.Port;
        if (reqHost.Contains(host) && reqPort==port)
        { hasRedirectDomain = true; break; }
      }
      return Task.FromResult(hasRedirectDomain);
    }

/// <summary>
/// Checks the return url of the client after logout
/// </summary>
/// <param name="requestedPostUri"></param>
/// <param name="client"></param>
/// <returns></returns>
    public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedPostUri, Client client)
    {
      var reqHost = new Uri(requestedPostUri).Host;
      bool hasPostRedirectDomain = false;
      var reqPort = new Uri(requestedPostUri).Port;
      foreach (string cUri in client.PostLogoutRedirectUris)
      {
        var uri = new Uri(cUri);
        var host = uri.Host;
        var port = uri.Port;
        if (reqHost.Contains(host) && reqPort==port )
        { hasPostRedirectDomain = true; break; }
      }
      return Task.FromResult(hasPostRedirectDomain);
    }
  }
}