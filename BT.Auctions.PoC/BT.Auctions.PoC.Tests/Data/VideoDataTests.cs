using System.Collections.Generic;
using System.Linq;
using BT.Auctions.Poc.Data;
using BT.Auctions.Poc.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BT.Auctions.PoC.Tests
{
    [TestClass]
    public class VideoDataTests
    {
        [TestMethod]
        public void GetVideosReturnsAListOfVideos()
        {
            VideoData videoData = new VideoData();
            IEnumerable<Video> videos = videoData.GetVideos();

            Assert.IsNotNull(videos);
            Assert.AreEqual(videos.Count(), 5);
        }
    }
}
