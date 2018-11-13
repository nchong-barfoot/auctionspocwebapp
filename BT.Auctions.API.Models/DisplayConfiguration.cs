using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT.Auctions.API.Models
{
    public class DisplayConfiguration : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisplayConfigurationId { get; set; }

        [ForeignKey("Display")]
        public int DisplayId { get; set; }
        public virtual Display Display { get; set; }

        [Required]
        public bool PlayVideo { get; set; }

        [Required]
        [DefaultValue(DisplayMode.Placeholder)]
        [EnumDataType(typeof(DisplayMode), ErrorMessage = "The provided Display Mode is not a recognised mode")]
        public DisplayMode DisplayMode { get; set; }

        [Required]
        public bool PlayAudio { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual ICollection<DisplayGroupConfigurations> DisplayGroupConfigurations { get; set; }

        [NotMapped]
        [JsonIgnore]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string CancellationReason { get; set; }
    }
}