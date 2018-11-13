using BT.Auctions.API.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT.Auctions.API.Models
{
    public class Media : ILogicPipelineObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaId { get; set; }
        [JsonIgnore]
        public string FileName { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        private DateTime? _createdOn { get; set; }
        public DateTime CreatedOn
        {
            get => _createdOn.HasValue ? _createdOn.Value : DateTime.UtcNow;
            set => _createdOn = value;
        }
        public DateTime? ModifiedOn { get; set; }

        [NotMapped]
        [JsonIgnore]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string CancellationReason { get; set; }
    }
}