using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Used to test the auction sesssion has a valid venue
    /// </summary>
    [TestClass]
    public class ValidateAuctionSessionHasValidVenueTest
    {
        private ValidateAuctionSessionHasValidVenue _validateAuctionSessionHasValidVenue;
        private Mock<IVenueRepository> _venueRepository;

        [TestInitialize]
        public void Initialize()
        {
            _venueRepository = new Mock<IVenueRepository>();
            _validateAuctionSessionHasValidVenue = new ValidateAuctionSessionHasValidVenue(_venueRepository.Object);
            var testVenueList = new List<Venue>
            {
                new Venue {
                    VenueId = 0,
                    Name = "Test Venue",
                    Region = "Auckland",
                    Street = "Auckland Street",
                    Suburb = "Auckland City"
                }
            };
            _venueRepository.Setup(v => v.GetVenueById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(testVenueList.Single(tv => tv.VenueId == id)));
        }

        [TestMethod]
        public async Task AuctionSession_Validate_AuctionSession_With_Valid_Venue()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                VenueId = 0,
                Name = "Test Auction Session"
            };

            //Action
            await _validateAuctionSessionHasValidVenue.Execute(testAuctionSession);

            //Result
            Assert.IsFalse(testAuctionSession.IsCancelled);
        }

        [TestMethod]
        public async Task AuctionSession_Validate_AuctionSession_With_Invalid_Venue_Not_In_System()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                VenueId = 1,
                Name = "Test Auction Session"
            };
            
            //Action
            await _validateAuctionSessionHasValidVenue.Execute(testAuctionSession);

            //Result
            Assert.IsTrue(testAuctionSession.IsCancelled);
            Assert.AreEqual($"No Venue was found matching ID {testAuctionSession.VenueId}. Cannot assign Auction Session to Venue.", testAuctionSession.CancellationReason);
        }

        [TestMethod]
        public async Task AuctionSession_Validate_AuctionSession_With_No_Venue_Provided()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                Name = "Test Auction Session"
            };

            //Action
            await _validateAuctionSessionHasValidVenue.Execute(testAuctionSession);

            //Result
            //System allows for auction sessions without venue Ids
            Assert.IsFalse(testAuctionSession.IsCancelled);
        }
    }
}