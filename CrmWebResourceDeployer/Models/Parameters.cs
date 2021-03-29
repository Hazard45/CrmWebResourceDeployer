namespace CrmWebResourceDeployer.Models
{
    public class Parameters
    {
        public string PublisherPrefix { get; set; }

        public string OrganizationServiceUrl { get; set; }

        public string WebResourcesRootPath { get; set; }

        public string[] IgnoreFiles { get; set; } = new string [] { };

        public bool PublishChanges { get; set; } = false;

        public bool IncludeExtensionInName { get; set; } = false;

        public bool IncludeSlashAfterPrefix { get; set; } = false;
    }
}