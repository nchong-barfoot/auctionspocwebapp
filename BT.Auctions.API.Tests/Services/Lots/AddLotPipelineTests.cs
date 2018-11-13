using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots
{
    /// <summary>
    /// Add Lot pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddLotPipelineTests
    {
        private AddLotPipeline _pipeline;
        private Mock<ILotRepository> _lotRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<ILogger<Lot>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Lot>>();
            _lotRepository = new Mock<ILotRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _pipeline = new AddLotPipeline(_logger.Object, _lotRepository.Object, _auctionSessionRepository.Object);
        }

        [TestMethod]
        public async Task Lot_Add_Pipeline_Valid_Lot()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                Address = "Test Address"
            };
            var lotList = new List<Lot>();

            _lotRepository.Setup(a => a.AddLot(It.IsAny<Lot>()))
                .Returns(Task.FromResult(testLot))
                .Callback<Lot>(v => lotList.Add(v));

            //Action
            var addedLot = await _pipeline.Execute(testLot);

            //Result
            Assert.IsFalse(addedLot.IsCancelled);
            Assert.AreEqual(1, lotList.Count);
        }

        [TestMethod]
        public async Task Lot_Add_Pipeline_Invalid_Lot_No_Address()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0
            };

            _lotRepository.Setup(a => a.AddLot(It.IsAny<Lot>()))
                .Returns(Task.FromResult(testLot));

            //Action
            var addedLot = await _pipeline.Execute(testLot);

            //Result
            Assert.IsTrue(addedLot.IsCancelled);
            Assert.AreEqual(addedLot.CancellationReason, "Lot address is required");
        }

        [TestMethod]
        public async Task Lot_Add_Pipeline_Invalid_Lot_Null_Values()
        {
            //Setup
            var lotList = new List<Lot>();

            _lotRepository.Setup(a => a.AddLot(It.IsAny<Lot>()))
                .Returns(Task.FromResult((Lot) null));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(null));

            //Result
            Assert.IsTrue(lotList.Count == 0);
        }
    }
}