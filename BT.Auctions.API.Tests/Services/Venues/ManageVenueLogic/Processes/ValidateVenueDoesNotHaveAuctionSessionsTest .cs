using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Validate Venue Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateVenueDoesNotHaveAuctionSessionsTest
    {
        private ValidateVenueDoesNotHaveAuctionSessions _validateVenueExists;
        private Mock<IAuctionSessionRepository> _auctionRepository;

        [TestInitialize]
        public void Initialize()
        {
            _auctionRepository = new Mock<IAuctionSessionRepository>();
            _validateVenueExists = new ValidateVenueDoesNotHaveAuctionSessions(_auctionRepository.Object);
        }

        /// <summary>
        /// Test to check if a venue with no name is not allowed through the pipeline
        /// Evaluating Process ValidateVenueHasName
        /// </summary>
        [TestMethod]
        public async Task Venue_Validate_No_Auction_Sessions_Present()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue"
            };

            var sessionList = new List<AuctionSession>();

            _auctionRepository.Setup(a => a.GetAuctionSessionsByVenueId(It.IsAny<int>()))
               .Returns<int>(venueId => Task.FromResult(sessionList.Where(s => s.VenueId == venueId)));

            //Action
            await _validateVenueExists.Execute(testVenue);

            //Result
            Assert.IsFalse(testVenue.IsCancelled);
        }

        /// <summary>
        /// validate auction session has valid venue
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Venue_Validate_Valid_Venue_Not_Found()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue"
            };

            var sessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    VenueId = 0,
                    AuctionSessionId = 0,
                    Name = "Auction Session 1"
                }
            };

            _auctionRepository.Setup(a => a.GetAuctionSessionsByVenueId(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(sessionList.Where(s => s.VenueId == venueId)));

            //Action
            await _validateVenueExists.Execute(testVenue);

            //Result
            Assert.IsTrue(testVenue.IsCancelled);
        }
    }
}