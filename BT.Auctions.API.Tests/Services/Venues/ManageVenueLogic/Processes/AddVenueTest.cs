using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Add venue pipeline process tests
    /// </summary>
    [TestClass]
    public class AddVenueTests
    {
        private AddVenue _addVenue;
        private Mock<IVenueRepository> _venueRepository;

        [TestInitialize]
        public void Initialize()
        {
            _venueRepository = new Mock<IVenueRepository>();
            _addVenue = new AddVenue(_venueRepository.Object);
        }

        [TestMethod]
        public async Task Venue_Add_Valid_Venue_To_Venue_List()
        {
            //Setup
            var testVenue = new Venue
            {
                Name = "Test Venue"
            };
            var venueList = new List<Venue>();
            _venueRepository.Setup(a => a.AddVenue(It.IsAny<Venue>()))
                .Returns(Task.FromResult(testVenue))
                .Callback<Venue>(v => venueList.Add(v));

            //Action
            await _addVenue.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.Count, 1);
        }

        [TestMethod]
        public async Task Venue_Add_Invalid_Venue()
        {
            //Setup
            Venue testVenue = null;
            var venueList = new List<Venue>();

            _venueRepository.Setup(a => a.AddVenue(It.IsAny<Venue>()))
                .Returns(Task.FromResult(testVenue))
                .Callback<Venue>(v => venueList.Add(v));

            //Action
            await _addVenue.Execute(testVenue);

            //Result
            Assert.AreEqual(venueList.Count, 1);
        }
    }
}