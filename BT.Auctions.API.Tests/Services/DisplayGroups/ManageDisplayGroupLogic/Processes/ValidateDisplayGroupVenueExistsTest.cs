using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BT.Auctions.API.Tests.Services.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Validate DisplayConfiguration Venue Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateDisplayGroupVenueExistsTest
    {
        private ValidateDisplayGroupVenueExists _validateDisplayGroupVenueExists;
        private Mock<IVenueRepository> _venueRepository;

        [TestInitialize]
        public void Initialize()
        {
            _venueRepository = new Mock<IVenueRepository>();
            _validateDisplayGroupVenueExists = new ValidateDisplayGroupVenueExists(_venueRepository.Object);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Validate_Valid_DisplayConfiguration_With_Venue_Found()
        {
            //Setup
            var testDisplayConfiguration = new DisplayGroup
            {
                VenueId = 0
            };

            var testSession = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0
                }
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.VenueId == auctionSessionId)));

            //Action
            await _validateDisplayGroupVenueExists.Execute(testDisplayConfiguration);

            //Result
            Assert.IsFalse(testDisplayConfiguration.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Validate_Valid_DisplayConfiguration_Without_Venue_Found()
        {
            //Setup
            var testDisplayConfiguration = new DisplayGroup
            {
                VenueId = 0
            };

            var testVenue = new List<Venue>
            {
                new Venue
                {
                    VenueId = 1
                }
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>()))
                .Returns<int>(venueId => Task.FromResult(testVenue.Single(i => i.VenueId == venueId)));

            //Action
            await _validateDisplayGroupVenueExists.Execute(testDisplayConfiguration);

            //Result
            Assert.IsTrue(testDisplayConfiguration.IsCancelled);
        }
    }
}