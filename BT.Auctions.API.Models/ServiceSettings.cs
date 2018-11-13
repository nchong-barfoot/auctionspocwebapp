namespace BT.Auctions.API.Models
{
    public class ServiceSettings
    {
        public string DisplaySecret { get; set; }
        public string AllowedPostIPAddresses { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccessKey { get; set; }
        public string StorageDefaultContainerName { get; set; }
    }
}
