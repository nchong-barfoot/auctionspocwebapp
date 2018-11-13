using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Validate LotDetail Lot Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateLotDetailLotExistsTest
    {
        private ValidateLotDetailLotExists _validateLotDetailLotExists;
        private Mock<ILotRepository> _lotRepository;

        [TestInitialize]
        public void Initialize()
        {
            _lotRepository = new Mock<ILotRepository>();
            _validateLotDetailLotExists = new ValidateLotDetailLotExists(_lotRepository.Object);
        }

        [TestMethod]
        public async Task LotDetail_Validate_Valid_LotDetail_With_Lot_Found()
        {
            //Setup
            var testLotDetail = new LotDetail()
            {
                LotId = 0
            };

            var testLots = new List<Lot>
            {
                new Lot
                {
                    LotId = 0
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
               .Returns<int>(lotId => Task.FromResult(testLots.Single(i => i.LotId == lotId)));

            //Action
            await _validateLotDetailLotExists.Execute(testLotDetail);

            //Result
            Assert.IsFalse(testLotDetail.IsCancelled);
        }

        [TestMethod]
        public async Task LotDetail_Validate_Valid_LotDetail_Without_Lot_Found()
        {
            //Setup
            var testLotDetail = new LotDetail()
            {
                LotId = 0
            };

            var testLots = new List<Lot>
            {
                new Lot
                {
                    LotId = 1
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns<int>(lotId => Task.FromResult(testLots.Single(i => i.LotId == lotId)));

            //Action
            await _validateLotDetailLotExists.Execute(testLotDetail);

            //Result
            Assert.IsTrue(testLotDetail.IsCancelled);
        }
    }
}