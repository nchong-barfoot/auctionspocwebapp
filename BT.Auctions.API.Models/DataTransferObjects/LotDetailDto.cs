using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class LotDetailDto
    {
        [ReadOnly(true)]
        public int? LotDetailId { get; set; }
        public int? LotId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}