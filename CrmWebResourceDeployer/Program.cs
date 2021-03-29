using System;
using System.Linq;
using CrmWebResourceDeployer.Managers;
using Microsoft.Crm.Sdk.Messages;

namespace CrmWebResourceDeployer
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parametersManager = new ParametersManager();
                var parameters = parametersManager.GetParameters(args);

                Console.WriteLine("Connecting to CRM service...");
                Console.WriteLine();
                var connectionManager = new ConnectionManager();
                var service = connectionManager.GetCrmService(parameters.OrganizationServiceUrl);

                var webResourceManager = new WebResourceManager(service);
                var webResources = webResourceManager.GetWebResourceFiles(parameters.WebResourcesRootPath, parameters);
                var crmWebResources = webResourceManager.GetCrmWebResources();

                if (webResources.Count > 0)
                {
                    foreach (var webResource in webResources)
                    {
                        try
                        {
                            webResourceManager.ValidateWebResourceName(webResource.Name, parameters.PublisherPrefix);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Web resource name is invalid: {webResource.Name} ({ex.Message})");
                            continue;
                        }

                        var existingWebResource = crmWebResources.FirstOrDefault(w => w.GetAttributeValue<string>("name") == webResource.Name);
                        if (existingWebResource != null)
                        {
                            webResourceManager.UpdateWebResource(webResource, existingWebResource.Id);
                            Console.WriteLine($"Web resource updated: {webResource.Name}");
                        }
                        else
                        {
                            webResourceManager.CreateWebResource(webResource);
                            Console.WriteLine($"Web resource created: {webResource.Name}");
                        }
                    }

                    if (parameters.PublishChanges)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Publishing customizations...");

                        var publishRequest = new PublishAllXmlRequest();
                        service.Execute(publishRequest);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("You chose NOT to publish changes");
                    }
                }
                else
                {
                    Console.WriteLine($"No web resources found in this folder: {parameters.WebResourcesRootPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Customizations NOT published due to error");
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Done");
                Console.ReadKey();
            }
        }
    }
}