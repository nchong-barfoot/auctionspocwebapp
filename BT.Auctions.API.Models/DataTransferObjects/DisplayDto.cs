using System.ComponentModel;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class DisplayDto
    {
        [ReadOnly(true)]
        public int? DisplayId { get; set; }
        public string DisplayName { get; set; }
        public int? VenueId { get; set; }
        public bool IsInUse { get; set; }
        public string VenueName { get; set; }
    }
}