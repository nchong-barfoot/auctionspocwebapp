using System.Collections.Generic;
using System.Linq;
using BT.Auctions.Poc.Data;
using BT.Auctions.Poc.Models;
using NUnit.Framework;

namespace BT.Auctions.PoC.Tests.Data
{
    [TestFixture]
    public class VideoDataTests
    {
        [Test]
        public void GetVideosReturnsAListOfVideos()
        {
            VideoData videoData = new VideoData();
            IEnumerable<Video> videos = videoData.GetVideos();

            Assert.IsNotNull(videos);
            Assert.AreEqual(videos.Count(), 5);
        }
    }
}
