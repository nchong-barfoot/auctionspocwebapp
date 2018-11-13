using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots
{
    /// <summary>
    /// Update Lot pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateLotPipelineTests
    {
        private UpdateLotPipeline _pipeline;
        private Mock<ILotRepository> _lotRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<ILogger<Lot>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Lot>>();
            _lotRepository = new Mock<ILotRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _pipeline = new UpdateLotPipeline(_logger.Object, _lotRepository.Object, _auctionSessionRepository.Object);
        }

        [TestMethod]
        public async Task Lot_Update_Pipeline_Valid_Lot()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                Address = "Updated Address"
            };
            var lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 0,
                    Address = "Address To Be Updated"
                }
            };

            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()))
                .Returns((int id, Lot lot) => Task.FromResult(lot))
                .Callback<int, Lot>((i, v) => lotList.Single(li => li.LotId == v.LotId).Address = v.Address);
            _lotRepository.Setup(a => a.GetLotDetailsById(It.IsAny<int>()))
               .Returns(Task.FromResult(testLot));


            //Action
            var updatedLot = await _pipeline.Execute(testLot);

            //Result
            Assert.IsFalse(updatedLot.IsCancelled);
            Assert.AreEqual("Updated Address", lotList[0].Address);
        }

        [TestMethod]
        public async Task Lot_Update_Pipeline_Invalid_Lot_No_Address()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0
            };
            var testAuctionSession = new AuctionSession
            {
                AuctionSessionId = 0
            };
            var lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 1
                }
            };

            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()));
            _lotRepository.Setup(a => a.GetLotDetailsById(It.IsAny<int>()))
               .Returns(Task.FromResult(testLot));
            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
                .Returns(Task.FromResult(testAuctionSession));

            //Action
            var updatedLot = await _pipeline.Execute(testLot);

            //Result
            Assert.AreEqual("Lot address is required",  updatedLot.CancellationReason);
        }
    }
}
