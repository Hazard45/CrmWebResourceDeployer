using System;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace CrmWebResourceDeployer.Managers
{
    public class ConnectionManager
    {
        public IOrganizationService GetCrmService(string organizationServiceUrl)
        {
            var uri = new Uri(organizationServiceUrl);
            var credentials = new ClientCredentials();
            credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            return new OrganizationServiceProxy(uri, null, credentials, null);
        }
    }
}
