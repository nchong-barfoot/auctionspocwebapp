namespace BT.Auctions.API.Models
{
    public class HubMethods
    {
        public HubMethod ChangeView { get; set; }
        public HubMethod ChangeMedia { get; set; }
        public HubMethod ChangeLot { get; set; }
        public HubMethod ConnectionFailure { get; set; }
        public HubMethod CompleteAuctionSession { get; set; }
        public HubMethod DisplayStatuses { get; set; }
        public HubMethod DisconnectClient { get; set; }
        public HubMethod EnableAudio { get; set; }
        public HubMethod IdentifyDisplay { get; set; }
        public HubMethod InitialiseSessionAndCache { get; set; }
        public HubMethod PauseMedia { get; set; }
        public HubMethod RefreshClients { get; set; }
        public HubMethod ResumeMedia { get; set; }
        public HubMethod RevertLotBid { get; set; }
        public HubMethod SetImage { get; set; }
        public HubMethod SetLotBid { get; set; }
        public HubMethod SetLotStatus { get; set; }
        public HubMethod SetPauseStatus { get; set; }
        public HubMethod SetStoreValues { get; set; }
        public HubMethod StartTimeSync { get; set; }
        public HubMethod SyncMessage { get; set; }
        public HubMethod UpdateAuctionSessions { get; set; }
        public HubMethod UpdateDisplayGroups { get; set; }
        public HubMethod ValidationMessage { get; set; }
    }
}
