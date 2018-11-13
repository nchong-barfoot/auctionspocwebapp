using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class DisplayConfigurationDto
    {
        [ReadOnly(true)]
        public int? DisplayConfigurationId { get; set; }
        public int? DisplayId { get; set; }
        public string DisplayName { get; set; }
        public bool? PlayVideo { get; set; }
        public bool? PlayAudio { get; set; }
        public bool? IsActive { get; set; }
        [EnumDataType(typeof(DisplayMode), ErrorMessage = "The provided Display Mode was not a recognised mode")]
        public DisplayMode? DisplayMode { get; set; }
        public IEnumerable<int> DisplayGroupIds { get; set; }
    }
}