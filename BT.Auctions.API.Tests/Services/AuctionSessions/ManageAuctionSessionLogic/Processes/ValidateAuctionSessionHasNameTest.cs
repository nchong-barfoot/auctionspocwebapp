using System;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BT.Auctions.API.Tests.Services.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Used to test the validate auctionSession has name pipeline process
    /// </summary>
    [TestClass]
    public class ValidateAuctionSessionHasNameTest 
    {
        private ValidateAuctionSessionHasName _validateAuctionSessionHasName;

        [TestInitialize]
        public void Initialize()
        {
            _validateAuctionSessionHasName = new ValidateAuctionSessionHasName();
        }

        [TestMethod]
        public async Task AuctionSession_Validate_Valid_AuctionSession_With_Name()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                AuctionSessionId = 0,
                Name = "Test AuctionSession"
            };

            //Action
            await _validateAuctionSessionHasName.Execute(testAuctionSession);

            //Result
            //Expected no exception but returned test auctionSession
        }

        [TestMethod]
        public async Task AuctionSession_Validate_Valid_AuctionSession_Not_Found()
        {
            //Setup
            AuctionSession testAuctionSession = null;

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _validateAuctionSessionHasName.Execute(testAuctionSession));

            //Result
            //Expected a null reference exception
        }

        [TestMethod]
        public async Task AuctionSession_Validate_Invalid_AuctionSession_No_Name()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                AuctionSessionId = 0
            };

            //Action
            await _validateAuctionSessionHasName.Execute(testAuctionSession);

            //Result
            Assert.IsTrue(testAuctionSession.IsCancelled);
        }
    }
}