using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Delete Venue pipeline process test
    /// </summary>
    [TestClass]
    public class DeleteVenueTest
    {
        private DeleteVenue _deleteVenue;
        private Mock<IVenueRepository> _venueRepository;

        [TestInitialize]
        public void Initialize()
        {
            _venueRepository = new Mock<IVenueRepository>();
            _deleteVenue = new DeleteVenue(_venueRepository.Object);
        }


        [TestMethod]
        public async Task Venue_Delete_Valid_Venue()
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

            _venueRepository.Setup(a => a.DeleteVenue(It.IsAny<Venue>())).Returns(Task.CompletedTask)
            .Callback<Venue>(v => venueList.Remove(venueList.Single(listItem => listItem.VenueId == v.VenueId)));


            //Action
            await _deleteVenue.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.Count, 0);
        }

        [TestMethod]
        public async Task Venue_Delete_Invalid_Venue()
        {
            //Setup
            Venue testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue"
            };
            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 1,
                    Name = "Test Venue Stays Here"
                }
            };

            _venueRepository.Setup(a => a.DeleteVenue(It.IsAny<Venue>())).Returns(Task.CompletedTask)
            .Callback<Venue>(v => venueList.Remove(venueList.Single(listItem => listItem.VenueId == v.VenueId)));

            //Action and Result
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _deleteVenue.Execute(testVenue));
        }

        [TestMethod]
        public async Task Venue_Delete_Invalid_Venue_No_Venues()
        {
            //Setup
            Venue testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue"
            };
            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.DeleteVenue(It.IsAny<Venue>())).Returns(Task.CompletedTask)
            .Callback<Venue>(v => venueList.Remove(venueList.Single(listItem => listItem.VenueId == v.VenueId)));

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _deleteVenue.Execute(testVenue));
        }
    }
}