



using System.ComponentModel.DataAnnotations;

namespace IDServer.Models {
  public class ClientTenantModel {
    public string SubDomainName
    {
      get; set;
    }

    public string TenantId
    {
      get; set;
    }
    public string TenantName
    {
      get; set;
    }
    public string LogoUrl
    {
      get; set;
    }
  }
}