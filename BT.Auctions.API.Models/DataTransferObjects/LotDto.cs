using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class LotDto
    {
        [ReadOnly(true)]
        public int? LotId { get; set; }
        public int? AuctionSessionId { get; set; }
        public int? Order { get; set; }
        public bool? ReserveMet { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Listing ID must be a positive number")]
        public int? ListingId { get; set; }
        public string Address { get; set; }
        public DateTime? PostponedDateTime { get; set; }
        public bool? PlusGST { get; set; }
        [EnumDataType(typeof(AuctionStatus), ErrorMessage = "The provided Auction Status is not a recognised status")]
        public AuctionStatus? AuctionStatus { get; set; }
        public string FeatureDescription { get; set; }
        public DateTime? SoldDate { get; set; }
        [Range(0, 999999999999.99, ErrorMessage = "Sold Price must be positive and below 999999999999.99")]
        public Decimal? SoldPrice { get; set; }
        public bool? IsSalePriceHidden { get; set; }
        public bool? IsPaused { get; set; }
        public IEnumerable<LotDetail> LotDetail { get; set; }
        public IEnumerable<ImageDto> Images { get; set; }
        public IEnumerable<BidDto> Bids { get; set; }
    }
}