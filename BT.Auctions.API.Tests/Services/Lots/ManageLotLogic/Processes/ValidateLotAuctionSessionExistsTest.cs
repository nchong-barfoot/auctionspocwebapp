using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Validate Lot Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateLotAuctionSessionExistsTest
    {
        private ValidateLotAuctionSessionExists _validateLotAuctionSessionExists;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;

        [TestInitialize]
        public void Initialize()
        {
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _validateLotAuctionSessionExists = new ValidateLotAuctionSessionExists(_auctionSessionRepository.Object);
        }

        [TestMethod]
        public async Task Lot_Validate_Valid_Lot_With_Action_Session_Found()
        {
            //Setup
            var testLot = new Lot()
            {
                AuctionSessionId = 0
            };

            var testSession = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 0
                }
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.AuctionSessionId == auctionSessionId)));

            //Action
            await _validateLotAuctionSessionExists.Execute(testLot);

            //Result
            Assert.IsFalse(testLot.IsCancelled);
        }

        [TestMethod]
        public async Task Lot_Validate_Valid_Lot_Without_Action_Session_Found()
        {
            //Setup
            var testLot = new Lot()
            {
                AuctionSessionId = 0
            };

            var testSession = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 1
                }
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
                .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.AuctionSessionId == auctionSessionId)));

            //Action
            await _validateLotAuctionSessionExists.Execute(testLot);

            //Result
            Assert.IsTrue(testLot.IsCancelled);
            Assert.AreEqual("No Auction Session was found matching ID 0", testLot.CancellationReason);
        }
    }
}