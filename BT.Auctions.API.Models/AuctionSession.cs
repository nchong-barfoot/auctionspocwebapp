using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BT.Auctions.API.Models
{
    public class AuctionSession : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuctionSessionId { get; set; }

        [ForeignKey("Venue")]
        public int? VenueId { get; set; }

        //Lazy loading has just been supported in core 2.1.0
        //https://docs.microsoft.com/en-us/ef/core/querying/related-data
        [JsonIgnore]
        public virtual Venue Venue { get; set; }


        [ForeignKey("DisplayGroup")]
        public int? DisplayGroupId { get; set; }
        public virtual DisplayGroup DisplayGroup { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }

        public string AuctionSessionAdmin { get; set; }

        [NotMapped]
        [JsonIgnore]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string CancellationReason { get; set; }

        [DefaultValue(false)]
        public bool IsInSession { get; set; }

        [NotMapped]
        public int LotCount
        {
            get => Lots.Count();
            private set { }
        }

        [JsonIgnore]
        public virtual IEnumerable<Lot> Lots { get; set; }
    }
}