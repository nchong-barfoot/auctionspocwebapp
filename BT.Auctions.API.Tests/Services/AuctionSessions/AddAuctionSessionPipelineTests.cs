using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.AuctionSessions
{
    /// <summary>
    /// Add AuctionSession pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddAuctionSessionPipelineTests
    {
        private AddAuctionSessionPipeline _pipeline;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<IVenueRepository> _venueRespository;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<ILogger<AuctionSession>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<AuctionSession>>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _venueRespository = new Mock<IVenueRepository>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _pipeline = new AddAuctionSessionPipeline(_logger.Object, _auctionSessionRepository.Object, _venueRespository.Object, _displayGroupRepository.Object);
        }
        
        [TestMethod]
        public async Task AuctionSession_Add_Pipeline_Valid_AuctionSession_With_Valid_Venue_Valid_StartDate()
        {
            //Setup
            var testVenueList = new List<Venue>
            { 
                new Venue {
                    VenueId = 0,
                    Name = "Test Venue",
                    Suburb = "Auckland City",
                    Region = "Auckland",
                    Street = "Auckland Street"
                }
            };

            var testSession = new AuctionSession
            {
                AuctionSessionId = 0,
                VenueId = 0,
                StartDate = new DateTime(2018, 03, 07),
                Name = "Test Auction Session 1"
            };

            var auctionSessionList = new List<AuctionSession>();

            _venueRespository.Setup(v => v.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(testVenueList.Single(v => v.VenueId == venueId)));

            _auctionSessionRepository.Setup(a => a.AddAuctionSession(It.IsAny<AuctionSession>()))
                .Returns<AuctionSession>(Task.FromResult)
                .Callback<AuctionSession>(session => auctionSessionList.Add(session));

            //Action
            await _pipeline.Execute(testSession);

            //Result
            Assert.AreEqual(auctionSessionList.Count, 1);
        }

        [TestMethod]
        public async Task AuctionSession_Add_Pipeline_Invalid_AuctionSession()
        {
            //Setup
            var testVenueList = new List<Venue>
            {
                new Venue {
                    VenueId = 0,
                    Name = "Test Venue",
                    Suburb = "Auckland City",
                    Region = "Auckland",
                    Street = "Auckland Street"
                }
            };

            var testSession = new AuctionSession
            {
                AuctionSessionId = 0,
                VenueId = 0,
                StartDate = new DateTime(2018, 03, 07),
                Name = "Test Auction Session 1"
            };

            var auctionSessionList = new List<AuctionSession>();

            _venueRespository.Setup(v => v.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(testVenueList.Single(v => v.VenueId == venueId)));

            _auctionSessionRepository.Setup(a => a.AddAuctionSession(It.IsAny<AuctionSession>()))
                .Returns<AuctionSession>(Task.FromResult)
                .Callback<AuctionSession>(session => auctionSessionList.Add(session));

            //Action
            await _pipeline.Execute(testSession);

            //Result
            Assert.AreEqual(auctionSessionList.Count, 1);
        }

        [TestMethod]
        public async Task AuctionSession_Add_Pipeline_Valid_AuctionSession_With_Valid_Venue_Valid_StartDate_Returns_Correct_Default_Flags()
        {
            //Setup
            var testVenueList = new List<Venue>
            {
                new Venue {
                    VenueId = 0,
                    Name = "Test Venue",
                    Suburb = "Auckland City",
                    Region = "Auckland",
                    Street = "Auckland Street"
                }
            };

            var testSession = new AuctionSession
            {
                AuctionSessionId = 0,
                VenueId = 0,
                StartDate = new DateTime(2018, 03, 07),
                Name = "Test Auction Session 1"
            };

            var auctionSessionList = new List<AuctionSession>();

            _venueRespository.Setup(v => v.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(testVenueList.Single(v => v.VenueId == venueId)));

            _auctionSessionRepository.Setup(a => a.AddAuctionSession(It.IsAny<AuctionSession>()))
                .Returns<AuctionSession>(Task.FromResult)
                .Callback<AuctionSession>(session => auctionSessionList.Add(session));

            //Action
            await _pipeline.Execute(testSession);

            //Result
            Assert.AreEqual(auctionSessionList.Count, 1);
            Assert.IsFalse(auctionSessionList.ElementAt(0).IsCancelled);
            Assert.IsFalse(auctionSessionList.ElementAt(0).IsInSession);
        }
    }
}