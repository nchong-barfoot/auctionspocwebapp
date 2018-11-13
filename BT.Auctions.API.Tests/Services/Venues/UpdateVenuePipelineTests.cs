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
    /// Update Venue pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateVenuePipelineTests
    {
        private UpdateVenuePipeline _pipeline;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<ILogger<Venue>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Venue>>();
            _venueRepository = new Mock<IVenueRepository>();
            _pipeline = new UpdateVenuePipeline(_logger.Object, _venueRepository.Object);
        }

        [TestMethod]
        public async Task Venue_Update_Pipeline_Valid_Venue()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Updated Venue",
                Suburb = "Auckland City",
                Region = "Auckland",
                Street = "Auckland Street"
            };
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue to update"
                }
            };

            _venueRepository.Setup(a => a.UpdateVenue(It.IsAny<int>(), It.IsAny<Venue>()))
                .Returns((int id, Venue venue) => Task.FromResult(venue))
                .Callback<int, Venue>((i, v) => venueList.Single(li => li.VenueId == v.VenueId).Name = v.Name);
            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns(Task.FromResult(testVenue));


            //Action
            await _pipeline.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.ElementAt(0).Name, "Updated Venue");
        }

        [TestMethod]
        public async Task Venue_Update_Pipeline_Invalid_Venue()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Updated Venue",
                Suburb = "Auckland City",
                Region = "Auckland",
                Street = "Auckland Street"
            };
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 1,
                    Name = "Test Venue to update"
                }
            };

            _venueRepository.Setup(a => a.UpdateVenue(It.IsAny<int>(), It.IsAny<Venue>()))
                .Returns((int id, Venue venue) => Task.FromResult(venue))
                .Callback<int, Venue>((i, v) => venueList.Single(li => li.VenueId == v.VenueId).Name = v.Name);
            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns(Task.FromResult(testVenue));


            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _pipeline.Execute(testVenue));

            //Result
        }
    }
}
