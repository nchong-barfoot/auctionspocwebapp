using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Used to test the lot has address pipeline process
    /// </summary>
    [TestClass]
    public class ValidateLotHasAddressTest
    {
        private ValidateLotHasAddress _validateLotHasAddress;

        [TestInitialize]
        public void Initialize()
        {
            _validateLotHasAddress = new ValidateLotHasAddress();
        }

        [TestMethod]
        public async Task Lot_Validate_Valid_Lot_With_Address()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0
            };

            //Action
            await _validateLotHasAddress.Execute(testLot);

            //Result
            //Expected no exception but returned test lot
        }

        [TestMethod]
        public async Task Lot_Validate_Valid_Lot_Not_Found()
        {
            //Setup
            Lot testLot = null;

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _validateLotHasAddress.Execute(testLot));

            //Result
            //Expected a null reference exception
        }

        [TestMethod]
        public async Task Lot_Validate_Invalid_Lot_Incomplete_Address()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0
            };

            //Action
            await  _validateLotHasAddress.Execute(testLot);

            //Result
            Assert.IsTrue(testLot.IsCancelled);
        }
    }
}