using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerEF
{
  public static class Config
  {
    public static IEnumerable<ApiResource> GetApiResources()
    {
      return new List<ApiResource>
    {
        new ApiResource("api1", "My API")
    };
    }
    public static IEnumerable<Client> GetClients()
    {
      return new List<Client>
    {
        new Client
        {
            ClientId = "client",

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // secret for authentication
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },

            // scopes that client has access to
            AllowedScopes = { "api1" }
        },
        // resource owner password grant client
        new Client
        {
            ClientId = "clientwithpassword",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "api1" }
        },
        // OpenID Connect implicit flow client (MVC)
        new Client
        {
            ClientId = "mvc",
            ClientName = "MVC Client",
            AllowedGrantTypes = GrantTypes.Implicit,

            // where to redirect to after login
            RedirectUris = { "http://localhost:52863/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "http://localhost:52863/signout-callback-oidc" },

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile
            }
        },
      // OpenID Connect Hybrid flow (MVC), gives auth token by browser and reference token to use by APIs
        new Client
        {
            ClientId = "mvchybrid",
            ClientName = "MVC Hybrid Client",
            AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
             ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
            // where to redirect to after login
            RedirectUris = { "http://localhost:52863/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "http://localhost:52863/signout-callback-oidc" },

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "api1"
            },
         AllowOfflineAccess= true
    }
  };
}
public static List<TestUser> GetUsers()
{
  return new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "password"
        },
        new TestUser
        {
            SubjectId = "2",
            Username = "bob",
            Password = "password"
        }
    };
}
//For Login UI, OpenId connect calls the user claima as IdentityResources
public static IEnumerable<IdentityResource> GetIdentityResources()
{
  return new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource { Name = "platformprop", UserClaims = new List<string> {"role"} }
    };
}

  }



}
