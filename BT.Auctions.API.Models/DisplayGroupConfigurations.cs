using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT.Auctions.API.Models
{
    public class DisplayGroupConfigurations
    {
        [ForeignKey("DisplayGroup")]
        [Required]
        public int DisplayGroupId { get; set; }

        public virtual DisplayGroup DisplayGroup { get; set; }

        [ForeignKey("DisplayConfiguration")]
        [Required]
        public int DisplayConfigurationId { get; set; }

        public virtual DisplayConfiguration DisplayConfiguration { get; set; }
    }
}