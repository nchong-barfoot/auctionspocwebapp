using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Update Display pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateDisplayTest 
    {
        private UpdateDisplay _updateDisplay;
        private Mock<IDisplayRepository> _displayRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayRepository = new Mock<IDisplayRepository>();
            _updateDisplay = new UpdateDisplay(_displayRepository.Object);
        }


        [TestMethod]
        public async Task Display_Update_Valid_Display()
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
                    DisplayName = "Test Display "
                }
            };

            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()))
                .Returns((int id, Display display) => Task.FromResult(display))
                .Callback<int, Display>((i, v) => displayList.Single(li => li.DisplayId == v.DisplayId).DisplayName = v.DisplayName);

            //Action
            await _updateDisplay.Execute(testDisplay);

            //Result
            Assert.AreEqual(displayList.ElementAt(0).DisplayName, "Updated Test Display ");
        }

        [TestMethod]
        public async Task Display_Update_Invalid_Display()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 1
            };

            var displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 0
                }
            };

            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()))
                .Returns((int id, Display display) => Task.FromResult(display))
                .Callback<int, Display>((i, v) => displayList.Single(li => li.DisplayId == v.DisplayId).DisplayName = v.DisplayName);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateDisplay.Execute(testDisplay));
        }

        [TestMethod]
        public async Task Display_Update_Invalid_Display_No_Displays()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0
            };

            var displayList = new List<Display>();

            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()))
                .Returns((int id, Display display) => Task.FromResult(display))
                .Callback<int, Display>((i, v) => displayList.Single(li => li.DisplayId == v.DisplayId).DisplayName = v.DisplayName);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateDisplay.Execute(testDisplay));
        }

        [TestMethod]
        public async Task Display_Update_No_Changes()
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
                    DisplayName = "Previous Display  DisplayName"
                }
            };

            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()))
                .Returns((int id, Display display) => Task.FromResult(display))
                .Callback<int, Display>((i, v) => displayList.Single(li => li.DisplayId == v.DisplayId).DisplayName = v.DisplayName);

            //Action
            await _updateDisplay.Execute(testDisplay);

            //Result
            Assert.AreEqual(displayList.ElementAt(0).DisplayName, "Updated Test Display ");
        }
    }
}