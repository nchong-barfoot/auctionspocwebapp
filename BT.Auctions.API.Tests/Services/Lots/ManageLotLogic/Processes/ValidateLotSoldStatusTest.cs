using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Used to test the lot has sold price and sold date set before allowing sold status
    /// </summary>
    [TestClass]
    public class ValidateLotSoldStatusTest
    {
        private ValidateLotSoldStatus _validateLotSoldStatus;

        [TestInitialize]
        public void Initialize()
        {
            _validateLotSoldStatus = new ValidateLotSoldStatus();
        }

        [TestMethod]
        public async Task Lot_Validate_Valid_Lot_With_Sold_Status_And_Sold_Price_And_Sold_Date_Set()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                SoldDate = new DateTime(2018, 07, 07),
                SoldPrice = 10,
                AuctionStatus = AuctionStatus.Sold
            };

            //Action
            await _validateLotSoldStatus.Execute(testLot);

            //Result
            Assert.IsFalse(testLot.IsCancelled);
        }

        [TestMethod]
        public async Task Lot_Validate_Invalid_Lot_With_Sold_Status_And_Sold_Price_Set_And_Sold_Date_Not_Set()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                SoldPrice = 10,
                AuctionStatus = AuctionStatus.Sold
            };

            //Action
            await _validateLotSoldStatus.Execute(testLot);

            //Result
            Assert.IsTrue(testLot.IsCancelled);
        }

        [TestMethod]
        public async Task Lot_Validate_Invalid_Lot_With_Sold_Status_And_Sold_Date_Set_And_Sold_Price_Not_Set()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                SoldDate = new DateTime(2018, 07, 07),
                AuctionStatus = AuctionStatus.Sold
            };

            //Action
            await _validateLotSoldStatus.Execute(testLot);

            //Result
            Assert.IsTrue(testLot.IsCancelled);
        }
    }
}