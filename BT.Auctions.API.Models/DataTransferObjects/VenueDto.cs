using System.ComponentModel;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class VenueDto
    {
        [ReadOnly(true)]
        public int? VenueId { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string Region { get; set; }
        public bool? OnSite { get; set; }
    }
}