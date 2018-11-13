using System;

namespace BT.Auctions.API.Models
{
    /// <summary>
    /// Model for SignalR Hub Media Objects
    /// </summary>
    public class HubMedia
    {
        public string MediaId { get; set; }
        public string StartDateTime { get; set; }
        public TimeSpan? ElapsedVideoTime { get; set; }
    }
}