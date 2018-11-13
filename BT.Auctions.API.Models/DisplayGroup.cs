using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BT.Auctions.API.Models
{
    public class DisplayGroup : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisplayGroupId { get; set; }

        [ForeignKey("Venue")]
        public int VenueId { get; set; }
        [JsonIgnore]
        public virtual Venue Venue { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<DisplayGroupConfigurations> DisplayGroupConfigurations { get; set; }

        public virtual ICollection<AuctionSession> AuctionSessions { get; set; }

        [NotMapped]
        [JsonIgnore]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string CancellationReason { get; set; }
        [NotMapped]
        public int DisplayCount
        {
            get => DisplayGroupConfigurations.Sum(g => g.DisplayConfiguration.DisplayGroupConfigurations.Count);
            private set { }
        }
        [NotMapped]
        public bool IsInUse { get; set; }
    }
}