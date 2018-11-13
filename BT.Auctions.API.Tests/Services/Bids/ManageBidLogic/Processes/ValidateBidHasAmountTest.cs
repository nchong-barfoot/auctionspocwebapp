using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Bids.ManageBidLogic.Processes
{
    /// <summary>
    /// Used to test the lot has address pipeline process
    /// </summary>
    [TestClass]
    public class ValidateLotDetailLotTest
    {
        private ValidateBidHasAmount _validateBidHasAmount;

        [TestInitialize]
        public void Initialize()
        {
            _validateBidHasAmount = new ValidateBidHasAmount();
        }

        [TestMethod]
        public async Task Bid_Validate_Valid_Bid_With_Amount()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0
            };

            //Action
            await _validateBidHasAmount.Execute(testBid);

            //Result
            //Expected no exception but returned test lot
        }

        [TestMethod]
        public async Task Bid_Validate_Valid_Bid_Not_Found()
        {
            //Setup
            Bid testBid = null;

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _validateBidHasAmount.Execute(testBid));

            //Result
            //Expected a null reference exception
        }

        [TestMethod]
        public async Task Bid_Validate_Invalid_Bid_Incomplete_Amount()
        {
            //Setup
            var testBid = new Bid
            {
                BidId = 0
            };

            //Action
            await  _validateBidHasAmount.Execute(testBid);

            //Result
            Assert.IsTrue(testBid.IsCancelled);
        }
    }
}