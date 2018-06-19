using System.Collections.Generic;
using BT.Auctions.Poc.Models;

namespace BT.Auctions.Poc.Data
{
    public interface IVideoData
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Video> GetVideos();
    }
}