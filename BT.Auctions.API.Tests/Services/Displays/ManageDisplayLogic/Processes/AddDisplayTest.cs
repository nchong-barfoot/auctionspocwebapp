using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Add display pipeline process tests
    /// </summary>
    [TestClass]
    public class AddDisplayTests
    {
        private AddDisplay _addDisplay;
        private Mock<IDisplayRepository> _displayRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayRepository = new Mock<IDisplayRepository>();
            _addDisplay = new AddDisplay(_displayRepository.Object);
        }

        [TestMethod]
        public async Task Display_Add_Valid_Display_To_Display_List()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0
            };
            var displayList = new List<Display>();
            _displayRepository.Setup(a => a.AddDisplay(It.IsAny<Display>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<Display>(v => displayList.Add(v));

            //Action
            await _addDisplay.Execute(testDisplay);

            //Result
            Assert.AreEqual(displayList.Count, 1);
        }

        [TestMethod]
        public async Task Display_Add_Invalid_Display()
        {
            //Setup
            var testDisplay = new Display();
            var displayList = new List<Display>();

            _displayRepository.Setup(a => a.AddDisplay(It.IsAny<Display>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<Display>(v => displayList.Add(v));

            //Action
            await _addDisplay.Execute(testDisplay);

            //Result
            Assert.AreEqual(displayList.Count, 1);
        }
    }
}