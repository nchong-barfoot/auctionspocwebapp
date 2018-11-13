using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Bids.ManageBidLogic.Processes
{
    /// <summary>
    /// Update Bid pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateBidTest 
    {
        private UpdateBid _updateBid;
        private Mock<IBidRepository> _bidRepository;

        [TestInitialize]
        public void Initialize()
        {
            _bidRepository = new Mock<IBidRepository>();
            _updateBid = new UpdateBid(_bidRepository.Object);
        }


        [TestMethod]
        public async Task Bid_Update_Valid_Bid()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0,
                Amount = 0
            };

            var bidList = new List<Bid>
            {
                new Bid
                {
                    BidId = 0,
                    Amount = 1
                }
            };

            _bidRepository.Setup(a => a.UpdateBid(It.IsAny<int>(), It.IsAny<Bid>()))
                .Returns((int id, Bid bid) => Task.FromResult(bid))
                .Callback<int, Bid>((i, v) => bidList.Single(li => li.BidId == v.BidId).Amount = v.Amount);

            //Action
            await _updateBid.Execute(testBid);

            //Result
            Assert.AreEqual(bidList.ElementAt(0).Amount, 0);
        }

        [TestMethod]
        public async Task Bid_Update_Invalid_Bid()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 1,
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
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateBid.Execute(testBid));
        }

        [TestMethod]
        public async Task Bid_Update_Invalid_Bid_No_Bids()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0,
                Amount = 1
            };

            var bidList = new List<Bid>();

            _bidRepository.Setup(a => a.UpdateBid(It.IsAny<int>(), It.IsAny<Bid>()))
                .Returns((int id, Bid bid) => Task.FromResult(bid))
                .Callback<int, Bid>((i, v) => bidList.Single(li => li.BidId == v.BidId).Amount = v.Amount);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateBid.Execute(testBid));
        }

        [TestMethod]
        public async Task Bid_Update_No_Changes()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0,
                Amount = 0
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
            await _updateBid.Execute(testBid);

            //Result
            Assert.AreEqual(bidList.ElementAt(0).Amount, 0);
        }
    }
}