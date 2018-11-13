using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT.Auctions.API.Models
{
    public class Display : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisplayId { get; set; }

        [Required]
        [ForeignKey("Venue")]
        public int VenueId { get; set; }
        [JsonIgnore]
        public virtual Venue Venue { get; set; }

        [Required]
        public string DisplayName { get; set; }
        
        public virtual ICollection<DisplayConfiguration> DisplayConfigurations { get; set; }

        [NotMapped]
        [JsonIgnore]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string CancellationReason { get; set; }
    }
}