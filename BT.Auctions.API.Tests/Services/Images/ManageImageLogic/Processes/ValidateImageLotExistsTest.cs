using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Images.ManageImageLogic.Processes
{
    /// <summary>
    /// Validate Image Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateImageLotExistsTest
    {
        private ValidateImageLotExists  _validateImageLotExists;
        private Mock<ILotRepository> _lotRepository;

        [TestInitialize]
        public void Initialize()
        {
            _lotRepository = new Mock<ILotRepository>();
            _validateImageLotExists = new ValidateImageLotExists(_lotRepository.Object);
        }

        [TestMethod]
        public async Task Image_Validate_Valid_Image_With_Lot_Found()
        {
            //Setup
            var testImage = new Image()
            {
                LotId = 0
            };

            var testSession = new List<Lot>
            {
                new Lot
                {
                    LotId = 0
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
               .Returns<int>(lotId => Task.FromResult(testSession.Single(i => i.LotId == lotId)));

            //Action
            await _validateImageLotExists.Execute(testImage);

            //Result
            Assert.IsFalse(testImage.IsCancelled);
        }

        [TestMethod]
        public async Task Image_Validate_Valid_Image_Without_Lot_Found()
        {
            //Setup
            var testImage = new Image()
            {
                LotId = 0
            };

            var testSession = new List<Lot>
            {
                new Lot
                {
                    LotId = 1
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns<int>(lotId => Task.FromResult(testSession.Single(i => i.LotId == lotId)));

            //Action
            await _validateImageLotExists.Execute(testImage);

            //Result
            Assert.IsTrue(testImage.IsCancelled);
        }
    }
}