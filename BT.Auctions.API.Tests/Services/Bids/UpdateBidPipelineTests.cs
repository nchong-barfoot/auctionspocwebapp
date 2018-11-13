using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Bids
{
    /// <summary>
    /// Update Bid pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateBidPipelineTests
    {
        private UpdateBidPipeline _pipeline;
        private Mock<ILotRepository> _lotRepository;
        private Mock<IBidRepository> _bidRepository;
        private Mock<ILogger<Bid>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Bid>>();
            _lotRepository = new Mock<ILotRepository>();
            _bidRepository = new Mock<IBidRepository>();
            _pipeline = new UpdateBidPipeline(_logger.Object, _bidRepository.Object, _lotRepository.Object);
        }

        [TestMethod]
        public async Task Bid_Update_Pipeline_Valid_Bid()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0,
                Amount = 1
            };
            var bidList = new List<Bid>
            {
                new Bid
                {
                    BidId = 0,
                    Amount = 0
                }
            };

            _bidRepository.Setup(a => a.UpdateBid(It.IsAny<int>(), It.IsAny<Bid>()))
                .Returns((int id, Bid bid) => Task.FromResult(bid))
                .Callback<int, Bid>((i, v) => bidList.Single(li => li.BidId == v.BidId).Amount = v.Amount);


            //Action
            var updatedBid = await _pipeline.Execute(testBid);

            //Result
            Assert.IsFalse(updatedBid.IsCancelled);
            Assert.AreEqual(1, bidList[0].Amount);
        }

        [TestMethod]
        public async Task Bid_Update_Pipeline_Invalid_Bid_No_Amount()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0
            };
            var bidList = new List<Bid>
            {
                new Bid
                {
                    BidId = 1
                }
            };

            _bidRepository.Setup(a => a.UpdateBid(It.IsAny<int>(), It.IsAny<Bid>()));

            //Action
            var updatedBid = await _pipeline.Execute(testBid);

            //Result
            Assert.IsTrue(updatedBid.IsCancelled);
        }
    }
}
