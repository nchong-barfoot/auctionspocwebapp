using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Validate Listing is Unique in Auction Session pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateLotHasUniqueListingIdInAuctionSessionTest
    {
        private ValidateLotHasUniqueListingIdInAuctionSession _validateLotHasUniqueListingIdInAuctionSession;

        [TestInitialize]
        public void Initialize()
        {
            _validateLotHasUniqueListingIdInAuctionSession = new ValidateLotHasUniqueListingIdInAuctionSession();
        }

        [TestMethod]
        public async Task Lot_Validate_Valid_Lot_With_Unique_ListingID_In_Action_Session_Found()
        {
            //Setup
            var testLots = new List<Lot>
            {
                new Lot
                {
                    AuctionSessionId = 0,
                    ListingId = 0
                },
                new Lot
                {
                    AuctionSessionId = 0,
                    ListingId = 1
                }
            };

            var testLot = new Lot()
            {
                AuctionSessionId = 0,
                ListingId = 2,
                AuctionSessionLots = testLots
            };

            //Action
            await _validateLotHasUniqueListingIdInAuctionSession.Execute(testLot);

            //Result
            Assert.IsFalse(testLot.IsCancelled);
        }

        [TestMethod]
        public async Task Lot_Validate_Invalid_Lot_With_Non_Unique_ListingID_In_Action_Session_Found()
        {
            //Setup
            var testLots = new List<Lot>
            {
                new Lot
                {
                    AuctionSessionId = 0,
                    ListingId = 0,
                    LotId = 0
                },
                new Lot
                {
                    AuctionSessionId = 0,
                    ListingId = 1,
                    LotId = 1
                }
            };

            var testLot = new Lot()
            {
                AuctionSessionId = 0,
                ListingId = 1,
                AuctionSessionLots = testLots
            };

            //Action
            await _validateLotHasUniqueListingIdInAuctionSession.Execute(testLot);

            //Result
            Assert.IsTrue(testLot.IsCancelled);
        }

        [TestMethod]
        public async Task Lot_Validate_Invalid_Lot_No_Action_Sessions_Found()
        {
            //Setup
            var testLots = new List<Lot>
            {
                new Lot
                {
                    AuctionSessionId = 0,
                    ListingId = 0,
                    LotId = 0
                }
            };

            var testLot = new Lot()
            {
                AuctionSessionId = 2,
                ListingId = 1,
                AuctionSessionLots = testLots
            };

            //Action
            await _validateLotHasUniqueListingIdInAuctionSession.Execute(testLot);

            //Result
            Assert.IsFalse(testLot.IsCancelled);
        }
    }
}