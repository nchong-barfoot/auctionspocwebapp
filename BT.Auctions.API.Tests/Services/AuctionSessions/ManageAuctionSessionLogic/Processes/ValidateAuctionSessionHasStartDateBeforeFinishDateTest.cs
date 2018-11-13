using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Used to test the validate auctionSession has name pipeline process
    /// </summary>
    [TestClass]
    public class ValidateAuctionSessionHasStartDateBeforeFinishDateTest
    {
        private ValidateAuctionSessionStartDateBeforeFinishDate _validateAuctionSessionStartDateBeforeFinishDate;

        [TestInitialize]
        public void Initialize()
        {
            _validateAuctionSessionStartDateBeforeFinishDate = new ValidateAuctionSessionStartDateBeforeFinishDate();
        }

        [TestMethod]
        public async Task AuctionSession_Validate_Valid_AuctionSession_With_StartDate_After_FinishDate()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                AuctionSessionId = 0,
                Name = "Test AuctionSession",
                StartDate = new DateTime(2018, 07, 06),
                FinishDate = new DateTime(2018, 07, 03)
            };

            //Action
            await _validateAuctionSessionStartDateBeforeFinishDate.Execute(testAuctionSession);

            //Result
            Assert.IsTrue(testAuctionSession.IsCancelled);
        }

        [TestMethod]
        public async Task AuctionSession_Validate_Valid_AuctionSession_With_StartDate_Before_FinishDate()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                AuctionSessionId = 0,
                Name = "Test AuctionSession",
                StartDate = new DateTime(2018, 07, 03),
                FinishDate = new DateTime(2018, 07, 06)
            };

            //Action
            await _validateAuctionSessionStartDateBeforeFinishDate.Execute(testAuctionSession);

            //Result
            Assert.IsFalse(testAuctionSession.IsCancelled);
        }

        [TestMethod]
        public async Task AuctionSession_Validate_Valid_AuctionSession_With_StartDate_Is_Null_FinishDate_Is_Valid()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                AuctionSessionId = 0,
                Name = "Test AuctionSession",
                FinishDate = new DateTime(2018, 07, 06)
            };

            //Action
            await _validateAuctionSessionStartDateBeforeFinishDate.Execute(testAuctionSession);

            //Result
            Assert.IsTrue(testAuctionSession.IsCancelled);
        }
    }
}