using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class AuctionSessionDto
    {
        [ReadOnly(true)]
        public int? AuctionSessionId { get; set; }
        public int? VenueId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? FinishDate { get; set; }
        public int? DisplayGroupId { get; set; }
        public string DisplayGroupName { get; set; }
        public bool? IsInSession { get; set; }
        public int LotCount { get; set; }
        public IEnumerable<LotDto> Lots { get; set; }
        public string AuctionSessionAdmin { get; set; }
    }
}