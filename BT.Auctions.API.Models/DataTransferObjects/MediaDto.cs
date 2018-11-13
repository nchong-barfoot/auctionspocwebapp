using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BT.Auctions.API.Models.DataTransferObjects
{
    public class MediaDto
    {
        [ReadOnly(true)]
        public int? MediaId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public IFormFile Data { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}