using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays
{
    /// <summary>
    /// Add Display pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddDisplayPipelineTests
    {
        private AddDisplayPipeline _pipeline;
        private Mock<IDisplayRepository> _displayRepository;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<ILogger<Display>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Display>>();
            _displayRepository = new Mock<IDisplayRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _pipeline = new AddDisplayPipeline(_logger.Object, _displayRepository.Object, _venueRepository.Object);
        }

        [TestMethod]
        public async Task Display_Add_Pipeline_Valid_Display()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Display "
            };
            var displayList = new List<Display>();

            _displayRepository.Setup(a => a.AddDisplay(It.IsAny<Display>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<Display>(v => displayList.Add(v));

            //Action
            var addedDisplay = await _pipeline.Execute(testDisplay);

            //Result
            Assert.IsFalse(addedDisplay.IsCancelled);
            Assert.AreEqual(1, displayList.Count);
        }

        [TestMethod]
        public async Task Display_Add_Pipeline_Invalid_Display_No_DisplayName()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0
            };

            //Action
            var addedDisplay = await _pipeline.Execute(testDisplay);

            //Result
            Assert.IsTrue(addedDisplay.IsCancelled);
        }

        [TestMethod]
        public async Task Display_Add_Pipeline_Invalid_Display_Null_Values()
        {
            //Setup
            var displayList = new List<Display>();

            _displayRepository.Setup(a => a.AddDisplay(It.IsAny<Display>()))
                .Returns(Task.FromResult((Display) null));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(null));

            //Result
            Assert.IsTrue(displayList.Count == 0);
        }
    }
}