using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CrmWebResourceDeployer.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CrmWebResourceDeployer.Managers
{
    public partial class WebResourceManager
    {
        private IOrganizationService _service;

        public WebResourceManager(IOrganizationService service)
        {
            _service = service;
        }

        public List<WebResource> GetWebResourceFiles(string webResourcesRootPath, Parameters parameters)
        {
            var directoryInfo = new DirectoryInfo(webResourcesRootPath);
            
            var validExtensions = new[] { ".css", ".xml", ".gif", ".htm", ".html", ".ico", ".jpg", ".png", ".js", ".xap", ".xsl", ".xslt" };
            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories).Where(f => validExtensions.Contains(f.Extension) && !parameters.IgnoreFiles.Contains(f.Name));

            var webResourcesInfo = new List<WebResource>();
            foreach (var file in files)
            {
                var type = file.Extension.Replace(".", string.Empty).ToLower();
                var name = file.FullName.Replace(webResourcesRootPath, string.Empty).Replace("\\", "/");
                if (!parameters.IncludeSlashAfterPrefix && name.StartsWith("/"))
                {
                    name = name.Substring(1);
                }
                if (!parameters.IncludeExtensionInName)
                {
                    var extension = file.Extension;
                    name = name.Substring(0, name.Length - extension.Length);
                }
                name = parameters.PublisherPrefix + "_" + name;

                var webResource = new WebResource
                {
                    FilePath = file.FullName,
                    Name = name,
                    DisplayName = file.Name,
                    Type = GetWebResourceTypeByExtension(type)
                };
                webResourcesInfo.Add(webResource);
            }
            return webResourcesInfo;
        }

        public List<Entity> GetCrmWebResources()
        {
            var query = new QueryExpression("webresource");
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
            query.Criteria.AddCondition("iscustomizable", ConditionOperator.Equal, true);
            query.ColumnSet.AddColumn("name");
            var webResources = _service.RetrieveMultiple(query);
            return webResources.Entities.ToList();
        }

        public void ValidateWebResourceName(string name, string publisherPrefix)
        {
            var invalidWebResourceNameRegex = new Regex("[^a-z0-9A-Z_\\./]|[/]{2,}", (RegexOptions.Compiled | RegexOptions.CultureInvariant));
            if (invalidWebResourceNameRegex.IsMatch(name))
            {
                throw new Exception("web resource name cannot contain spaces or hyphens. They must be alphanumeric and contain underscore characters, periods, and non-consecutive forward slash characters");
            }
            if (name.Remove(0, publisherPrefix.Length + 1).Length > 100)
            {
                throw new Exception("web resource name must be <= 100 characters");
            }
        }

        public void UpdateWebResource(WebResource webResource, Guid webResourceId)
        {
            var webResourceEntity = new Entity("webresource", webResourceId);
            webResourceEntity["content"] = GetWebResourceContent(webResource.FilePath);
            webResourceEntity["displayname"] = webResource.DisplayName;
            _service.Update(webResourceEntity);
        }

        public void CreateWebResource(WebResource webResource)
        {
            var webResourceEntity = new Entity("webresource");
            webResourceEntity["name"] = webResource.Name;
            webResourceEntity["content"] = GetWebResourceContent(webResource.FilePath);
            webResourceEntity["displayname"] = webResource.DisplayName;
            webResourceEntity["webresourcetype"] = new OptionSetValue(webResource.Type);
            _service.Create(webResourceEntity);
        }

        private string GetWebResourceContent(string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var binaryData = new byte[stream.Length];

            stream.Read(binaryData, 0, (int)stream.Length);
            stream.Close();

            return Convert.ToBase64String(binaryData, 0, binaryData.Length);
        }

        private int GetWebResourceTypeByExtension(string extension)
        {
            var type = 0;
            switch (extension.ToLower())
            {
                case "htm":
                case "html":
                    type = 1;
                    break;
                case "css":
                    type = 2;
                    break;
                case "js":
                    type = 3;
                    break;
                case "xml":
                    type = 4;
                    break;
                case "png":
                    type = 5;
                    break;
                case "jpg":
                case "jpeg":
                    type = 4;
                    break;
                case "gif":
                    type = 7;
                    break;
                case "xap":
                    type = 8;
                    break;
                case "xsl":
                    type = 9;
                    break;
                case "ico":
                    type = 10;
                    break;
                case "svg":
                    type = 11;
                    break;
                case "resx":
                    type = 12;
                    break;
                default:
                    break;
            }
            return type;
        }
    }
}