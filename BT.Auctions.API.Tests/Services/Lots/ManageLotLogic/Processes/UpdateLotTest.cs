using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Update Lot pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateLotTest 
    {
        private UpdateLot _updateLot;
        private Mock<ILotRepository> _lotRepository;

        [TestInitialize]
        public void Initialize()
        {
            _lotRepository = new Mock<ILotRepository>();
            _updateLot = new UpdateLot(_lotRepository.Object);
        }


        [TestMethod]
        public async Task Lot_Update_Valid_Lot()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                Address = "Updated Lot"
            };

            var lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 0,
                    Address = "Test Lot"
                }
            };

            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()))
                .Returns((int id, Lot lot) => Task.FromResult(lot))
                .Callback<int, Lot>((i, v) => lotList.Single(li => li.LotId == v.LotId).Address = v.Address);

            //Action
            await _updateLot.Execute(testLot);

            //Result
            Assert.AreEqual(lotList.ElementAt(0).Address, "Updated Lot");
        }

        [TestMethod]
        public async Task Lot_Update_Invalid_Lot()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 1,
                Address = "Updated Lot"
            };

            var lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 0,
                    Address = "Test Lot"
                }
            };

            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()))
                .Returns((int id, Lot lot) => Task.FromResult(lot))
                .Callback<int, Lot>((i, v) => lotList.Single(li => li.LotId == v.LotId).Address = v.Address);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateLot.Execute(testLot));
        }

        [TestMethod]
        public async Task Lot_Update_Invalid_Lot_No_Lots()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                Address = "Updated Lot"
            };

            var lotList = new List<Lot>();

            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()))
                .Returns((int id, Lot lot) => Task.FromResult(lot))
                .Callback<int, Lot>((i, v) => lotList.Single(li => li.LotId == v.LotId).Address = v.Address);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateLot.Execute(testLot));
        }

        [TestMethod]
        public async Task Lot_Update_No_Changes()
        {
            //Setup
            var testLot = new Lot
            {
                LotId = 0,
                Address = "Test Lot"
            };

            var lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 0,
                    Address = "Test Lot"
                }
            };

            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()))
                .Returns((int id, Lot lot) => Task.FromResult(lot))
                .Callback<int, Lot>((i, v) => lotList.Single(li => li.LotId == v.LotId).Address = v.Address);

            //Action
            await _updateLot.Execute(testLot);

            //Result
            Assert.AreEqual(lotList.ElementAt(0).Address, "Test Lot");
        }
    }
}