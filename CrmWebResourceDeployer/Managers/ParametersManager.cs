using System;
using CrmWebResourceDeployer.Models;

namespace CrmWebResourceDeployer.Managers
{
    public class ParametersManager
    {
        public Parameters GetParameters(string[] args)
        {
            var parameters = new Parameters();
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    var paramName = GetParameterName(arg);
                    var paramValue = arg.Substring($"-{paramName}:".Length);
                    switch (paramName)
                    {
                        case "OrganizationServiceUrl":
                        case "o":
                            parameters.OrganizationServiceUrl = paramValue;
                            break;
                        case "PublisherPrefix":
                        case "x":
                            parameters.PublisherPrefix = paramValue;
                            break;
                        case "WebResourcesRootPath":
                        case "w":
                            parameters.WebResourcesRootPath = paramValue;
                            break;
                        case "IgnoreFiles":
                        case "i":
                            parameters.IgnoreFiles = paramValue.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            break;
                        case "PublishChanges":
                        case "p":
                            parameters.PublishChanges = Convert.ToBoolean(paramValue);
                            break;
                        case "IncludeExtensionInName":
                        case "e":
                            parameters.IncludeExtensionInName = Convert.ToBoolean(paramValue);
                            break;
                        case "IncludeSlashAfterPrefix":
                        case "s":
                            parameters.IncludeSlashAfterPrefix = Convert.ToBoolean(paramValue);
                            break;
                        case "":
                            throw new Exception("Empty parameters are not allowed");
                        default:
                            throw new Exception($"Parameter with such name is not allowed: {paramName}");
                    }
                }
            }
            else
            {
                ShowHelp();

                Console.WriteLine("Provide parameters manually:");

                Console.Write("Organization Service Url: ");
                parameters.OrganizationServiceUrl = Console.ReadLine().Trim();

                Console.Write("Publisher Prefix: ");
                parameters.PublisherPrefix = Console.ReadLine().Trim();

                Console.Write("Web Resources Root Path: ");
                parameters.WebResourcesRootPath = Console.ReadLine().Trim();

                Console.Write("Ignore Files: ");
                parameters.IgnoreFiles = Console.ReadLine().Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                Console.Write("Publish Changes: ");
                parameters.PublishChanges = Convert.ToBoolean(Console.ReadLine());

                Console.Write("Include Extension In Name: ");
                parameters.IncludeExtensionInName = Convert.ToBoolean(Console.ReadLine());

                Console.Write("Include Slash In Name: ");
                parameters.IncludeSlashAfterPrefix = Convert.ToBoolean(Console.ReadLine());
            }
            CheckRequiredkParameters(parameters);
            return parameters;
        }

        private string GetParameterName(string arg)
        {
            var name = string.Empty;
            if (arg.StartsWith("-"))
            {
                var dotsIndex = arg.IndexOf(":");
                name = arg.Substring(1, dotsIndex - 1);
            }
            return name;
        }

        private void CheckRequiredkParameters(Parameters parameters)
        {
            var message = string.Empty;
            if (string.IsNullOrWhiteSpace(parameters.PublisherPrefix))
            {
                message += $"'PublisherPrefix' parameter is empty{Environment.NewLine}";
            }
            if (string.IsNullOrWhiteSpace(parameters.OrganizationServiceUrl))
            {
                message += $"'OrganizationServiceUrl' parameter is empty{Environment.NewLine}";
            }
            if (string.IsNullOrWhiteSpace(parameters.WebResourcesRootPath))
            {
                message += $"'WebResourcesRootPath' parameter is empty{Environment.NewLine}";
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                throw new Exception(message);
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine(
@"CrmWebResourceDeployer.exe -- deploying web resources to Dynamics 365

Parameters:
-OrganizationServiceUrl: [or -o:]    CRM organization service URL (required)
-PublisherPrefix: [or -x:]           CRM publisher prefix ('_' is not needed) (required)
-WebResourcesRootPath: [or -w:]      Folder path where to search web resources (required)
-IgnoreFiles: [or -i:]               File names to ignore (not required, separated by '|' symbol)
-PublishChanges: [or -p:]            Publish customizations in CRM (not required, default is 'False')
-IncludeExtensionInName: [or -e:]    Include file extension in web resource name (not required, default is 'False')
-IncludeSlashAfterPrefix: [or -s:]   Include slash in web resource name after publisher prefix (not required, default is 'False')");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}