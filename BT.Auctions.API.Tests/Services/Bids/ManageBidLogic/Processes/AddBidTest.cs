using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Bids.ManageBidLogic.Processes
{
    /// <summary>
    /// Add bid pipeline process tests
    /// </summary>
    [TestClass]
    public class AddBidTests
    {
        private AddBid _addBid;
        private Mock<IBidRepository> _bidRepository;

        [TestInitialize]
        public void Initialize()
        {
            _bidRepository = new Mock<IBidRepository>();
            _addBid = new AddBid(_bidRepository.Object);
        }

        [TestMethod]
        public async Task Bid_Add_Valid_Bid_To_Bid_List()
        {
            //Setup
            var testBid = new Bid
            {
                Amount = 1
            };
            var bidList = new List<Bid>();
            _bidRepository.Setup(a => a.AddBid(It.IsAny<Bid>()))
                .Returns(Task.FromResult(testBid))
                .Callback<Bid>(v => bidList.Add(v));

            //Action
            await _addBid.Execute(testBid);

            //Result
            Assert.AreEqual(bidList.Count, 1);
        }

        [TestMethod]
        public async Task Bid_Add_Invalid_Bid()
        {
            //Setup
            Bid testBid = null;
            var bidList = new List<Bid>();

            _bidRepository.Setup(a => a.AddBid(It.IsAny<Bid>()))
                .Returns(Task.FromResult(testBid))
                .Callback<Bid>(v => bidList.Add(v));

            //Action
            await _addBid.Execute(testBid);

            //Result
            Assert.AreEqual(bidList.Count, 1);
        }
    }
}