using System.Collections.Generic;
using System.Linq;
using BT.Auctions.Poc.Data;
using BT.Auctions.Poc.Models;

namespace BT.Auctions.PoC.Services
{
    public class VideoService : IVideoService
    {
        /// <summary>
        /// Gets the video by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public Video GetVideoById(int Id)
        {
            VideoData videoData = new VideoData();
            IEnumerable<Video> videos = videoData.GetVideos();

            return videos.FirstOrDefault(v => v.Id == Id);
        }
    }
}
