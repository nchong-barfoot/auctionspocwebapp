using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Used to test the validate venue has name pipeline process
    /// </summary>
    [TestClass]
    public class ValidateVenueHasNameTest 
    {
        private ValidateVenueHasName _validateVenueHasName;

        [TestInitialize]
        public void Initialize()
        {
            _validateVenueHasName = new ValidateVenueHasName();
        }

        [TestMethod]
        public async Task Venue_Validate_Valid_Venue_With_Name()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0,
                Name = "Test Venue"
            };

            //Action
            await _validateVenueHasName.Execute(testVenue);

            //Result
            //Expected no exception but returned test venue
        }

        [TestMethod]
        public async Task Venue_Validate_Valid_Venue_Not_Found()
        {
            //Setup
            Venue testVenue = null;

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _validateVenueHasName.Execute(testVenue));

            //Result
            //Expected a null reference exception
        }

        [TestMethod]
        public async Task Venue_Validate_Invalid_Venue_No_Name()
        {
            //Setup
            var testVenue = new Venue
            {
                VenueId = 0
            };

            //Action
            await _validateVenueHasName.Execute(testVenue);

            //Result
            Assert.IsTrue(testVenue.IsCancelled);
        }
    }
}