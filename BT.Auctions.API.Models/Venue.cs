using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;

namespace BT.Auctions.API.Models
{
    public class Venue : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VenueId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Suburb { get; set; }
        [Required]
        public string Region { get; set; }

        //OnSite not required for Phase One
        [DefaultValue(false)]
        public bool OnSite { get; set; }

        /// <summary>
        /// Will indicate if a logic pipeline has been canceled. 
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }

        /// <summary>
        /// If a logic pipeline is canceled. The reason will be shown here. 
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string CancellationReason { get; set; }
    }
}