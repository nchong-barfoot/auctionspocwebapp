using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays
{
    /// <summary>
    /// Delete Display pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class DeleteDisplayPipelineTests
    {
        private DeleteDisplayPipeline _pipeline;
        private Mock<IDisplayRepository> _displayRepository;
        private Mock<ILogger<Display>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Display>>();
            _displayRepository = new Mock<IDisplayRepository>();
            _pipeline = new DeleteDisplayPipeline(_logger.Object, _displayRepository.Object);
        }

        [TestMethod]
        public async Task Display_Delete_Pipeline_Valid_Display()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Test Display"
            };
            var displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 0,
                    DisplayName = "Test Display to remove"
                }
            };

            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplay));
            _displayRepository.Setup(a => a.DeleteDisplay(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<int>(id => displayList.Remove(displayList.Single(listItem => listItem.DisplayId == id)));


            //Action
            await _pipeline.Execute(testDisplay);

            //Result
            Assert.AreEqual(displayList.Count, 0);
        }

        [TestMethod]
        public async Task Display_Delete_Pipeline_Invalid_Display()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Test Display"
            };
            var displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 1,
                    DisplayName = "Test Display to stay"
                }
            };

            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>())).Throws(new InvalidOperationException());

            //Action
            await _pipeline.Execute(testDisplay);

            //Result
            Assert.IsTrue(testDisplay.IsCancelled);
        }
    }
}
