using System.ComponentModel;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class ImageDto
    {
        [ReadOnly(true)]
        public int? ImageId { get; set; }
        public int? LotId { get; set; }
        public int? Order { get; set; }
        public string Url { get; set; }
        public bool? IsDisplayed { get; set; }
    }
}