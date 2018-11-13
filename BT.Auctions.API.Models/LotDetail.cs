using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT.Auctions.API.Models
{
    public class LotDetail : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LotDetailId { get; set; }

        [ForeignKey("Lot")]
        [Required]
        public int LotId { get; set; }
        [JsonIgnore]
        public virtual Lot Lot { get; set; }

        [Required]
        public string Key { get; set; }
        public string Value { get; set; }

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