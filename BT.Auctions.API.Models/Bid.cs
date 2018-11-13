using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT.Auctions.API.Models
{
    public class Bid : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BidId { get; set; }

        [ForeignKey("Lot")]
        public int LotId { get; set; }
        [JsonIgnore]
        public virtual Lot Lot { get; set; }

        public decimal Amount { get; set; }
        public bool IsRejected { get; set; }
        public bool ReserveMet { get; set; }
        private DateTime? _createdOn { get; set; }
        public DateTime CreatedOn
        {
            get => _createdOn.HasValue ? _createdOn.Value : DateTime.UtcNow;
            set => _createdOn = value;
        }
        public DateTime? ModifiedOn { get; set; }

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