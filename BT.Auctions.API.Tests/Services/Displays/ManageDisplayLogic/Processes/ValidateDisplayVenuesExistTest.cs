using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Validate Display Venue Exists Tests
    /// </summary>
    [TestClass]
    public class ValidateDisplayVenuesExistTest
    {
        private ValidateDisplayVenueExists _validateDisplayVenuesExist;
        private Mock<IVenueRepository> _venueRepository;

        [TestInitialize]
        public void Initialize()
        {
            _venueRepository = new Mock<IVenueRepository>();
            _validateDisplayVenuesExist = new ValidateDisplayVenueExists(_venueRepository.Object);
        }

        [TestMethod]
        public async Task Display_Validate_Valid_Display_With_Venue_Found_In_System()
        {
            //Setup
            var testDisplay = new Display()
            {
                DisplayId = 0,
                VenueId = 0
            };

            var testVenues = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0
                }
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns<int>(id => Task.FromResult(testVenues.Single(i => i.VenueId == id)));

            //Action
            await _validateDisplayVenuesExist.Execute(testDisplay);

            //Result
            Assert.IsFalse(testDisplay.IsCancelled);
        }

        [TestMethod]
        public async Task Display_Validate_Invalid_Display_With_Venue_Not_In_System()
        {
            //Setup
            var testDisplay = new Display()
            {
                DisplayId = 0,
                VenueId = 0
            };

            var testVenues = new List<Venue>
            {
                new Venue
                {
                    VenueId = 1
                }
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns<int>(id => Task.FromResult(testVenues.Single(i => i.VenueId == id)));

            //Action
            await _validateDisplayVenuesExist.Execute(testDisplay);

            //Result
            Assert.IsTrue(testDisplay.IsCancelled);
        }
    }
}