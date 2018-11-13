namespace BT.Auctions.API.Models
{
    /// <summary>
    /// Model for SignalR Hub Client Objects
    /// </summary>
    public class HubClient
    {
        public string ConnectionId { get; set; }
        public string TimeZone { get; set; }
        public HubClientTimeStamp TimeStamp { get; set; }
        public bool IsCached { get; set; }
    }
}

