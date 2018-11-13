using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Venues
{
    /// <summary>
    /// Add Venue pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddVenuePipelineTests
    {
        private AddVenuePipeline _pipeline;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<ILogger<Venue>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Venue>>();
            _venueRepository = new Mock<IVenueRepository>();
            _pipeline = new AddVenuePipeline(_logger.Object, _venueRepository.Object);
        }
        
        [TestMethod]
        public async Task Venue_Add_Pipeline_Valid_Venue()
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
            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.AddVenue(It.IsAny<Venue>()))
                .Returns(Task.FromResult(testVenue))
                .Callback<Venue>(v => venueList.Add(v));

            //Action
            await _pipeline.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.Count, 1);
        }

        [TestMethod]
        public async Task Venue_Add_Pipeline_Invalid_Venue()
        {
            //Setup
            Venue testVenue = null;
            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.AddVenue(It.IsAny<Venue>()))
                .Returns(Task.FromResult(testVenue))
                .Callback<Venue>(v => venueList.Add(v));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(testVenue));

            //Result
            Assert.IsTrue(venueList.Count == 0);
        }

        [TestMethod]
        public async Task Venue_Add_Pipeline_With_Invalid_Venue_Without_Name()
        {
            //Setup
            Venue testVenue = new Venue
            {
                Name = "",
                Suburb = "Auckland City",
                Region = "Auckland",
                Street = "Auckland Street"
            };
            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.AddVenue(It.IsAny<Venue>()))
                .Returns(Task.FromResult(testVenue))
                .Callback<Venue>(v => venueList.Add(v));

            //Action
            var addedVenue = await _pipeline.Execute(testVenue);

            //Result
            Assert.IsTrue(addedVenue.IsCancelled);
            Assert.IsTrue(venueList.Count == 0);
        }

        [TestMethod]
        public async Task Venue_Add_Pipeline_With_Invalid_Venue_Without_Address()
        {
            //Setup
            Venue testVenue = new Venue
            {
                Name = "Venue Name",
                Suburb = "",
                Region = "Auckland",
                Street = "Auckland Street"
            };
            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.AddVenue(It.IsAny<Venue>()))
                .Returns(Task.FromResult(testVenue))
                .Callback<Venue>(v => venueList.Add(v));

            //Action
            var addedVenue = await _pipeline.Execute(testVenue);

            //Result
            Assert.IsTrue(addedVenue.IsCancelled);
            Assert.IsTrue(venueList.Count == 0);
        }
    }
}