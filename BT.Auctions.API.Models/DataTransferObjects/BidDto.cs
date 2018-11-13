using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class BidDto
    {
        [ReadOnly(true)]
        public int? BidId { get; set; }
        public int? LotId { get; set; }
        [Range(0, 999999999999.99, ErrorMessage = "Invalid Bid Amount. Value must be equal to or below 999999999999.99")]
        public decimal? Amount { get; set; }
        public bool? IsRejected { get; set; }
        public bool? ReserveMet { get; set; }
    }
}