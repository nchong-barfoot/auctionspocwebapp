using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BT.Auctions.API.Tests.Services.AuctionSessions
{
    /// <summary>
    /// Update Venue pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateAuctionSessionPipelineTests
    {
        private UpdateAuctionSessionPipeline _pipeline;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<ILogger<AuctionSession>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<AuctionSession>>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _pipeline = new UpdateAuctionSessionPipeline(_logger.Object, _auctionSessionRepository.Object,
                _venueRepository.Object, _displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task AuctionSession_Update_Pipeline_Valid_Auction_Session()
        {
            //Setup
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            var testAuctionSession = new AuctionSession
            {
                VenueId = 0,
                AuctionSessionId = 0,
                Name = "New Auction",
                StartDate = new DateTime(2018, 07, 03)
            };

            var testAuctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    VenueId = 0,
                    AuctionSessionId = 0,
                    Name = "Previous Auction"
                }
            };

            _auctionSessionRepository.Setup(a => a.UpdateAuctionSession(It.IsAny<int>(), It.IsAny<AuctionSession>()))
                .Returns((int id, AuctionSession session) => Task.FromResult(testAuctionSession))
                .Callback<int, AuctionSession>((i, s) =>
                    testAuctionSessionList.Single(li => li.AuctionSessionId == s.AuctionSessionId).Name = s.Name);

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(venueList.Single(v => v.VenueId == venueId)));

            //Action
            await _pipeline.Execute(testAuctionSession);

            //Result
            Assert.IsFalse(testAuctionSession.IsCancelled);
            Assert.AreEqual("New Auction", testAuctionSessionList.ElementAt(0).Name);
        }

        [TestMethod]
        public async Task AuctionSession_Update_Pipeline_Invalid_Auction_Session_No_Name()
        {
            //Setup
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            var testAuctionSession = new AuctionSession
            {
                VenueId = 0,
                AuctionSessionId = 0,
                Name = "",
                StartDate = new DateTime(2018, 07, 03)
            };

            var testAuctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    VenueId = 0,
                    AuctionSessionId = 0,
                    Name = "Previous Auction"
                }
            };

            _auctionSessionRepository.Setup(a => a.UpdateAuctionSession(It.IsAny<int>(), It.IsAny<AuctionSession>()));
            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()));

            //Action
            await _pipeline.Execute(testAuctionSession);

            //Result
            Assert.IsTrue(testAuctionSession.IsCancelled);
            Assert.AreEqual(testAuctionSession.CancellationReason, "Auction Session Name was not provided");
            //update never completed, expect previous name to be the same
            Assert.AreEqual("Previous Auction", testAuctionSessionList.ElementAt(0).Name);
        }


        [TestMethod]
        public async Task AuctionSession_Update_Pipeline_Invalid_Auction_Session_Invalid_Not_Found_VenueId_Provided()
        {
            //Setup
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            var testAuctionSession = new AuctionSession
            {
                VenueId = 1,
                AuctionSessionId = 0,
                Name = "New Auction",
                StartDate = new DateTime(2018, 07, 03)
            };

            var testAuctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    VenueId = 1,
                    AuctionSessionId = 0,
                    Name = "Previous Auction"
                }
            };

            _auctionSessionRepository.Setup(a => a.UpdateAuctionSession(It.IsAny<int>(), It.IsAny<AuctionSession>()));
            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(venueList.Single(v => v.VenueId == venueId)));

            //Action
            await _pipeline.Execute(testAuctionSession);

            //Result
            Assert.IsTrue(testAuctionSession.IsCancelled);
            Assert.AreEqual(testAuctionSession.CancellationReason,
                "No Venue was found matching ID 1. Cannot assign Auction Session to Venue.");
            //update never completed, expect previous name to be the same
            Assert.AreEqual("Previous Auction", testAuctionSessionList.ElementAt(0).Name);
        }

        [TestMethod]
        public async Task AuctionSession_Update_Pipeline_Valid_Auction_Session_Valid_False_IsInSession_Flag_Provided()
        {
            //Setup
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            var testAuctionSession = new AuctionSession
            {
                VenueId = 0,
                AuctionSessionId = 0,
                Name = "New Auction",
                IsInSession = false
            };

            var testAuctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    VenueId = 0,
                    AuctionSessionId = 0,
                    Name = "Previous Auction",
                    IsInSession = true
                }
            };

            _auctionSessionRepository.Setup(a => a.UpdateAuctionSession(It.IsAny<int>(), It.IsAny<AuctionSession>()))
                .Returns((int id, AuctionSession session) => Task.FromResult(testAuctionSession))
                .Callback<int, AuctionSession>((i, s) =>
                    testAuctionSessionList.Single(li => li.AuctionSessionId == s.AuctionSessionId).IsInSession = s.IsInSession);

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(venueList.Single(v => v.VenueId == venueId)));

            //Action
            await _pipeline.Execute(testAuctionSession);

            //Result
            Assert.IsFalse(testAuctionSession.IsInSession);
            Assert.IsFalse(testAuctionSessionList.ElementAt(0).IsInSession);
        }

        [TestMethod]
        public async Task AuctionSession_Update_Pipeline_Valid_Auction_Session_Valid_True_IsInSession_Flag_Provided()
        {
            //Setup
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            var testAuctionSession = new AuctionSession
            {
                VenueId = 0,
                AuctionSessionId = 0,
                Name = "New Auction",
                IsInSession = true
            };

            var testAuctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    VenueId = 0,
                    AuctionSessionId = 0,
                    Name = "Previous Auction",
                    IsInSession = false
                }
            };

            _auctionSessionRepository.Setup(a => a.UpdateAuctionSession(It.IsAny<int>(), It.IsAny<AuctionSession>()))
                .Returns((int id, AuctionSession session) => Task.FromResult(testAuctionSession))
                .Callback<int, AuctionSession>((i, s) =>
                    testAuctionSessionList.Single(li => li.AuctionSessionId == s.AuctionSessionId).IsInSession = s.IsInSession);

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(venueList.Single(v => v.VenueId == venueId)));

            //Action
            await _pipeline.Execute(testAuctionSession);

            //Result
            Assert.IsTrue(testAuctionSession.IsInSession);
            Assert.IsTrue(testAuctionSessionList.ElementAt(0).IsInSession);
        }
    }
}
