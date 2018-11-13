using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Bids.ManageBidLogic.Processes
{
    /// <summary>
    /// Validate Bid Lot Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateBidLotExistsTest
    {
        private ValidateBidLotExists _validateBidLotExists;
        private Mock<ILotRepository> _lotRepository;

        [TestInitialize]
        public void Initialize()
        {
            _lotRepository = new Mock<ILotRepository>();
            _validateBidLotExists = new ValidateBidLotExists(_lotRepository.Object);
        }

        [TestMethod]
        public async Task Bid_Validate_Valid_Bid_With_Lot_Found()
        {
            //Setup
            var testBid = new Bid()
            {
                LotId = 0
            };

            var testLots = new List<Lot>
            {
                new Lot
                {
                    LotId = 0
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
               .Returns<int>(lotId => Task.FromResult(testLots.Single(i => i.LotId == lotId)));

            //Action
            await _validateBidLotExists.Execute(testBid);

            //Result
            Assert.IsFalse(testBid.IsCancelled);
        }

        [TestMethod]
        public async Task Bid_Validate_Valid_Bid_Without_Lot_Found()
        {
            //Setup
            var testBid = new Bid()
            {
                LotId = 0
            };

            var testLots = new List<Lot>
            {
                new Lot
                {
                    LotId = 1
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns<int>(lotId => Task.FromResult(testLots.Single(i => i.LotId == lotId)));

            //Action
            await _validateBidLotExists.Execute(testBid);

            //Result
            Assert.IsTrue(testBid.IsCancelled);
        }
    }
}