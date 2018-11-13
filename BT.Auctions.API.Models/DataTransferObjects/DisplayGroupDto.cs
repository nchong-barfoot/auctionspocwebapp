using System.Collections.Generic;
using System.ComponentModel;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class DisplayGroupDto
    {
        [ReadOnly(true)]
        public int? DisplayGroupId { get; set; }
        public int? VenueId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<int> DisplayConfigurationIds { get; set; }
        public IEnumerable<int> AuctionSessionIds { get; set; }
        public bool IsInUse { get; set; }
        public int DisplayCount { get; set; }
    }
}