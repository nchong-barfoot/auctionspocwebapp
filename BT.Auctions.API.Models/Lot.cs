using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;

namespace BT.Auctions.API.Models
{
    public class Lot : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LotId { get; set; }

        [ForeignKey("AuctionSession")]
        public int AuctionSessionId { get; set; }
        [JsonIgnore]
        public virtual AuctionSession AuctionSession { get; set; }
        public int Order { get; set; }
        [DefaultValue(false)]
        public bool ReserveMet { get; set; }
        [Required]
        public int ListingId { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime? PostponedDateTime { get; set; }
        [DefaultValue(false)]
        public bool PlusGST { get; set; }
        [Required]
        [DefaultValue(AuctionStatus.None)]
        [EnumDataType(typeof(AuctionStatus), ErrorMessage = "The provided Auction Status is not a recognised status")]
        public AuctionStatus AuctionStatus { get; set; }
        public string FeatureDescription { get; set; }
        public DateTime? SoldDate { get; set; }
        public Decimal? SoldPrice { get; set; }

        [DefaultValue(false)]
        public bool? IsSalePriceHidden { get; set; }
        
        [DefaultValue(false)]
        public bool? IsPaused { get; set; }

        //Mapping collection to LotDetail
        public virtual ICollection<LotDetail> LotDetail { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        [NotMapped]
        public virtual IEnumerable<Lot> AuctionSessionLots { get; set; }

        [NotMapped]
        //JsonIgnore required to hide it from SwaggerUI
        [JsonIgnore]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string CancellationReason { get; set; }
    }
}