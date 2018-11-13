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
    public class ValidateVenueExistsTest 
    {
        private ValidateVenueExists _validateVenueExists;
        private Mock<IVenueRepository> _venueRepository;

        [TestInitialize]
        public void Initialize()
        {
            _venueRepository = new Mock<IVenueRepository>();
            _validateVenueExists = new ValidateVenueExists(_venueRepository.Object);
        }

        /// <summary>
        /// Test to check if a venue with no name is not allowed through the pipeline
        /// Evaluating Process ValidateVenueHasName
        /// </summary>
        [TestMethod]
        public async Task Venue_Validate_Valid_Venue()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue"
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns(Task.FromResult(testVenue));

            //Action
            await _validateVenueExists.Execute(testVenue);

            //Result
            //Expected no exception but returned test venue
        }

        /// <summary>
        /// validate valid venue that is not found within the system.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Venue_Validate_Valid_Venue_Not_Found()
        {
            //Setup
            Venue testVenue = new Venue
            {
                VenueId = -1,
                Name = "Test Venue"
            };

            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns(Task.FromResult(testVenue)).Callback<int>((id) => venueList.Single(v => v.VenueId == id));

            //Action
            await _validateVenueExists.Execute(testVenue);

            //Result
            Assert.IsTrue(testVenue.IsCancelled);
        }
    }
}