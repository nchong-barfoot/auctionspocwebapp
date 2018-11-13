namespace BT.Auctions.API.Models
{
    /// <summary>
    /// Model for SignalR Hub Client Timestamp Objects
    /// </summary>
    public class HubClientTimeStamp
    {
        public double? MaxDifference { get; set; }
        public double MinDifference { get; set; }
        public double? TimingDifference { get; set; }
        public string Timing { get; set; }
    }
}

