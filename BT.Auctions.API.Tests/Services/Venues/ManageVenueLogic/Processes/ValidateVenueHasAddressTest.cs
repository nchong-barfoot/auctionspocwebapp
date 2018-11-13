using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Used to test the venue has address pipeline process
    /// </summary>
    [TestClass]
    public class ValidateVenueHasAddressTest
    {
        private ValidateVenueHasAddress _validateVenueHasAddress;

        [TestInitialize]
        public void Initialize()
        {
            _validateVenueHasAddress = new ValidateVenueHasAddress();
        }

        [TestMethod]
        public async Task Venue_Validate_Valid_Venue_With_Address()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue",
                Region = "Auckland",
                Street = "Auckland Street",
                Suburb = "Auckland City"
            };

            //Action
            await _validateVenueHasAddress.Execute(testVenue);

            //Result
            //Expected no exception but returned test venue
        }

        [TestMethod]
        public async Task Venue_Validate_Valid_Venue_Not_Found()
        {
            //Setup
            Venue testVenue = null;

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _validateVenueHasAddress.Execute(testVenue));

            //Result
            //Expected a null reference exception
        }

        [TestMethod]
        public async Task Venue_Validate_Invalid_Venue_Incomplete_Address()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue",
                Region = "Auckland",
                Street = "Auckland Street"
            };

            //Action
            await  _validateVenueHasAddress.Execute(testVenue);

            //Result
            Assert.IsTrue(testVenue.IsCancelled);
        }
    }
}