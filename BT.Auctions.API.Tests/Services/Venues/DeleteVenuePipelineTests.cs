using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Venues
{
    /// <summary>
    /// Delete Venue pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class DeleteVenuePipelineTests
    {
        private DeleteVenuePipeline _pipeline;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<ILogger<Venue>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Venue>>();
            _venueRepository = new Mock<IVenueRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _pipeline = new DeleteVenuePipeline(_logger.Object, _venueRepository.Object, _auctionSessionRepository.Object);
        }

        [TestMethod]
        public async Task Venue_Delete_Pipeline_Valid_Venue()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue",
                Suburb = "Auckland City",
                Region = "Auckland",
                Street = "Auckland Street"
            };
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue to remove"
                }
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns(Task.FromResult(testVenue));
            _venueRepository.Setup(a => a.DeleteVenue(It.IsAny<Venue>())).Returns(Task.CompletedTask)
            .Callback<Venue>(v => venueList.Remove(venueList.Single(listItem => listItem.VenueId == v.VenueId)));


            //Action
            await _pipeline.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.Count, 0);
        }

        [TestMethod]
        public async Task Venue_Delete_Pipeline_Invalid_Venue()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue",
                Suburb = "Auckland City",
                Region = "Auckland",
                Street = "Auckland Street"
            };
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 1,
                    Name = "Test Venue to stay"
                }
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns(Task.FromResult(testVenue));
            _venueRepository.Setup(a => a.DeleteVenue(It.IsAny<Venue>())).Returns(Task.CompletedTask)
            .Callback<Venue>(v => venueList.Remove(venueList.Single(listItem => listItem.VenueId == v.VenueId)));


            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _pipeline.Execute(testVenue));

            //Result
            //invalid operation exception thrown as venue does not exist
        }
    }
}
