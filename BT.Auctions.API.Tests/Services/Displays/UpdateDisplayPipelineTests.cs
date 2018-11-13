using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays
{
    /// <summary>
    /// Update Display pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateDisplayPipelineTests
    {
        private UpdateDisplayPipeline _pipeline;
        private Mock<IDisplayRepository> _displayRepository;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<ILogger<Display>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Display>>();
            _displayRepository = new Mock<IDisplayRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _pipeline = new UpdateDisplayPipeline(_logger.Object, _displayRepository.Object, _venueRepository.Object);
        }

        [TestMethod]
        public async Task Display_Update_Pipeline_Valid_Display()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Updated Test Display "
            };
            var displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 0,
                    DisplayName = "Previous Test Display "
                }
            };

            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()))
                .Returns((int id, Display display) => Task.FromResult(display))
                .Callback<int, Display>((i, v) => displayList.Single(li => li.DisplayId == v.DisplayId).DisplayName = v.DisplayName);
            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplay));


            //Action
            var updatedDisplay = await _pipeline.Execute(testDisplay);

            //Result
            Assert.IsFalse(updatedDisplay.IsCancelled);
            Assert.AreEqual(displayList[0].DisplayName, "Updated Test Display ");
        }

        [TestMethod]
        public async Task Display_Update_Pipeline_Invalid_Display_No_Venue()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0,

            };
            var displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 0
                }
            };

            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()));
            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplay));
            

            //Action
            var updatedDisplay = await _pipeline.Execute(testDisplay);

            //Result
            Assert.IsTrue(updatedDisplay.IsCancelled);
        }
    }
}
