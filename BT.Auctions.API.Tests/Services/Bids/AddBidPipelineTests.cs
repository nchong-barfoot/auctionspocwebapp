using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Bids
{
    /// <summary>
    /// Add Bid pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddBidPipelineTests
    {
        private AddBidPipeline _pipeline;
        private Mock<IBidRepository> _bidRepository;
        private Mock<ILotRepository> _lotRepository;
        private Mock<ILogger<Bid>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Bid>>();
            _bidRepository = new Mock<IBidRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _pipeline = new AddBidPipeline(_logger.Object, _bidRepository.Object, _lotRepository.Object);
        }

        [TestMethod]
        public async Task Bid_Add_Pipeline_Valid_Bid()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0,
                Amount = 1
            };
            var bidList = new List<Bid>();

            _bidRepository.Setup(a => a.AddBid(It.IsAny<Bid>()))
                .Returns(Task.FromResult(testBid))
                .Callback<Bid>(v => bidList.Add(v));

            //Action
            var addedBid = await _pipeline.Execute(testBid);

            //Result
            Assert.IsFalse(addedBid.IsCancelled);
            Assert.AreEqual(1, bidList.Count);
        }

        [TestMethod]
        public async Task Bid_Add_Pipeline_Invalid_Bid_No_Lot()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0,
                Amount = 1
            };

            var testLots = new List<Lot>
            {
                new Lot
                {
                    LotId = 1
                }
            };

            _bidRepository.Setup(a => a.AddBid(It.IsAny<Bid>()))
                .Returns(Task.FromResult(testBid));
            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns<int>(lotId => Task.FromResult(testLots.Single(i => i.LotId == lotId)));

            //Action
            var addedBid = await _pipeline.Execute(testBid);

            //Result
            Assert.IsTrue(addedBid.IsCancelled);
        }

        [TestMethod]
        public async Task Bid_Add_Pipeline_Invalid_Bid_Null_Values()
        {
            //Setup
            var bidList = new List<Bid>();

            _bidRepository.Setup(a => a.AddBid(It.IsAny<Bid>()))
                .Returns(Task.FromResult((Bid) null));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(null));

            //Result
            Assert.IsTrue(bidList.Count == 0);
        }
    }
}