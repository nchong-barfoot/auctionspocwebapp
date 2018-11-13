using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BT.Auctions.API.Tests.Services.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Update Venue pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateAuctionSessionTest 
    {
        private UpdateVenue _updateVenue;
        private Mock<IVenueRepository> _venueRepository;

        [TestInitialize]
        public void Initialize()
        {
            _venueRepository = new Mock<IVenueRepository>();
            _updateVenue = new UpdateVenue(_venueRepository.Object);
        }


        [TestMethod]
        public async Task Venue_Update_Valid_Venue()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Updated Venue"
            };

            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            _venueRepository.Setup(a => a.UpdateVenue(It.IsAny<int>(), It.IsAny<Venue>()))
                .Returns((int id, Venue venue) => Task.FromResult(venue))
                .Callback<int, Venue>((i, v) => venueList.Single(li => li.VenueId == v.VenueId).Name = v.Name);

            //Action
            await _updateVenue.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.ElementAt(0).Name, "Updated Venue");
        }

        [TestMethod]
        public async Task Venue_Update_Invalid_Venue()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 1,
                Name = "Updated Venue"
            };

            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            _venueRepository.Setup(a => a.UpdateVenue(It.IsAny<int>(), It.IsAny<Venue>()))
                .Returns((int id, Venue venue) => Task.FromResult(venue))
                .Callback<int, Venue>((i, v) => venueList.Single(li => li.VenueId == v.VenueId).Name = v.Name);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateVenue.Execute(testVenue));
        }

        [TestMethod]
        public async Task Venue_Update_Invalid_Venue_No_Venues()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Updated Venue"
            };

            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.UpdateVenue(It.IsAny<int>(), It.IsAny<Venue>()))
                .Returns((int id, Venue venue) => Task.FromResult(venue))
                .Callback<int, Venue>((i, v) => venueList.Single(li => li.VenueId == v.VenueId).Name = v.Name);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateVenue.Execute(testVenue));
        }

        [TestMethod]
        public async Task Venue_Update_No_Changes()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue"
            };

            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue"
                }
            };

            _venueRepository.Setup(a => a.UpdateVenue(It.IsAny<int>(), It.IsAny<Venue>()))
                .Returns((int id, Venue venue) => Task.FromResult(venue))
                .Callback<int, Venue>((i, v) => venueList.Single(li => li.VenueId == v.VenueId).Name = v.Name);

            //Action
            await _updateVenue.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.ElementAt(0).Name, "Test Venue");
        }
    }
}