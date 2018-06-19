using System.Collections.Generic;
using BT.Auctions.Poc.Models;

namespace BT.Auctions.Poc.Data
{
    public class VideoData : IVideoData
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Video> GetVideos()
        {
            return new List<Video>
            {
                new Video
                {
                    Id = 1,
                    Title = "Test1"
                },
                new Video
                {
                    Id = 2,
                    Title = "Test2"
                },
                new Video
                {
                    Id = 3,
                    Title = "Test3"
                },
                new Video
                {
                    Id = 4,
                    Title = "Test4"
                },
                new Video
                {
                    Id = 5,
                    Title = "Test5"
                },
                new Video
                {
                    Id = 6,
                    Title = "Test6"
                }
            };
        }
    }
}
