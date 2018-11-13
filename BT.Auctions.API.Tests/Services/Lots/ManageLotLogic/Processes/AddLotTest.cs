using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Add lot pipeline process tests
    /// </summary>
    [TestClass]
    public class AddLotTests
    {
        private AddLot _addLot;
        private Mock<ILotRepository> _lotRepository;

        [TestInitialize]
        public void Initialize()
        {
            _lotRepository = new Mock<ILotRepository>();
            _addLot = new AddLot(_lotRepository.Object);
        }

        [TestMethod]
        public async Task Lot_Add_Valid_Lot_To_Lot_List()
        {
            //Setup
            var testLot = new Lot
            {
                Address = "Test Lot"
            };
            var lotList = new List<Lot>();
            _lotRepository.Setup(a => a.AddLot(It.IsAny<Lot>()))
                .Returns(Task.FromResult(testLot))
                .Callback<Lot>(v => lotList.Add(v));

            //Action
            await _addLot.Execute(testLot);

            //Result
            Assert.AreEqual(lotList.Count, 1);
        }

        [TestMethod]
        public async Task Lot_Add_Invalid_Lot()
        {
            //Setup
            Lot testLot = null;
            var lotList = new List<Lot>();

            _lotRepository.Setup(a => a.AddLot(It.IsAny<Lot>()))
                .Returns(Task.FromResult(testLot))
                .Callback<Lot>(v => lotList.Add(v));

            //Action
            await _addLot.Execute(testLot);

            //Result
            Assert.AreEqual(lotList.Count, 1);
        }
    }
}