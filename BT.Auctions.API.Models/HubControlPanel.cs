using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Models
{
    /// <summary>
    /// Hub Control Panel Model for managing current stat and values of the control panel
    /// to refresh the connected client in the event of a crash
    /// </summary>
    public class HubControlPanel
    { 
        public string ConnectionId { get; set; }
        public int? VenueId { get; set; }
        public int? DisplayGroupId { get; set; }
        public int? CurrentLotId { get; set; }
        public AuctionSession AuctionSession { get; set; }
        public AuctionSessionState AuctionSessionState { get; set; }
        public HubClient HubClient { get; set; }
    }
}