using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Validate Lot order is unique against the auction session
    /// </summary>
    [TestClass]
    public class ValidateLotOrderIsUniqueInAuctionSessionTest
    {
        private ValidateLotOrderIsUniqueInAuctionSession _validateOrderIsUniqueInAuctionSession;

        [TestInitialize]
        public void Initialize()
        {
            _validateOrderIsUniqueInAuctionSession = new ValidateLotOrderIsUniqueInAuctionSession();
        }

        [TestMethod]
        public async Task Lot_Validate_Valid_And_Unique_Order_Number_Provided()
        {
            //Setup
            var testLot = new Lot()
            {
                AuctionSessionId = 0,
                Order = 1
            };

            var testLots = new List<Lot>
            {
                new Lot
                {
                    AuctionSessionId = 0,
                    LotId = 0,
                    Order = 0
                }
            };

            //Action
            await _validateOrderIsUniqueInAuctionSession.Execute(testLot);

            //Result
            Assert.IsFalse(testLot.IsCancelled);
        }

        [TestMethod]
        public async Task Lot_Validate_Invalid_Unique_Order_Number_Provided()
        {
            //Setup
            var testLots = new List<Lot>
            {
                new Lot
                {
                    AuctionSessionId = 0,
                    LotId = 0,
                    Order = 0
                }
            };

            var testLot = new Lot()
            {
                AuctionSessionId = 0,
                Order = 0,
                LotId = 1,
                AuctionSessionLots = testLots
            };

            //Action
            await _validateOrderIsUniqueInAuctionSession.Execute(testLot);

            //Result
            Assert.IsTrue(testLot.IsCancelled);
        }
    }
}