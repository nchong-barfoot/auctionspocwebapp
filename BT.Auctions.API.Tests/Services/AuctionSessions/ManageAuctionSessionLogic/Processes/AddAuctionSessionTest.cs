using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Add auctionSession pipeline process tests
    /// </summary>
    [TestClass]
    public class AddAuctionSessionTests
    {
        private AddAuctionSession _addAuctionSession;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;

        [TestInitialize]
        public void Initialize()
        {
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _addAuctionSession = new AddAuctionSession(_auctionSessionRepository.Object);
        }

        [TestMethod]
        public async Task AuctionSession_Add_Valid_AuctionSession_To_AuctionSession_List()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                Name = "Test Auction Session"
            };
            var auctionSessionList = new List<AuctionSession>();

            _auctionSessionRepository.Setup(a => a.AddAuctionSession(It.IsAny<AuctionSession>()))
                .Returns(Task.FromResult(testAuctionSession))
                .Callback<AuctionSession>(v => auctionSessionList.Add(v));

            //Action
            await _addAuctionSession.Execute(testAuctionSession);

            //Result
            Assert.AreEqual(auctionSessionList.Count, 1);
        }

        [TestMethod]
        public async Task AuctionSession_Add_Invalid_AuctionSession_To_AuctionSession_List()
        {
            //Setup
            var testAuctionSession = new AuctionSession
            {
                Name = "Test Auction Session"
            };
            var auctionSessionList = new List<AuctionSession>();

            _auctionSessionRepository.Setup(a => a.AddAuctionSession(It.IsAny<AuctionSession>()))
                .Returns(Task.FromResult(testAuctionSession))
                .Callback<AuctionSession>(v => auctionSessionList.Add(v));

            //Action
            await _addAuctionSession.Execute(testAuctionSession);

            //Result
            Assert.AreEqual(auctionSessionList.Count, 1);
        }
    }
}