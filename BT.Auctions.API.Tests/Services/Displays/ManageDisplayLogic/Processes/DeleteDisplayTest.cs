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
    /// Delete Display pipeline process test
    /// </summary>
    [TestClass]
    public class DeleteDisplayTest
    {
        private DeleteDisplay _deleteDisplay;
        private Mock<IDisplayRepository> _displayRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayRepository = new Mock<IDisplayRepository>();
            _deleteDisplay = new DeleteDisplay(_displayRepository.Object);
        }


        [TestMethod]
        public async Task Display_Delete_Valid_Display()
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
                    DisplayName = "Test Display"
                }
            };

            _displayRepository.Setup(a => a.DeleteDisplay(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<int>(id => displayList.Remove(displayList.Single(listItem => listItem.DisplayId == id)));

            //Action
            await _deleteDisplay.Execute(testDisplay);

            //Result
            Assert.AreEqual(displayList.Count, 0);
        }

        [TestMethod]
        public async Task Display_Delete_Invalid_Display()
        {
            //Setup
            Display testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Test Display"
            };
            var displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 1,
                    DisplayName = "Test Display Stays Here"
                }
            };

            _displayRepository.Setup(a => a.DeleteDisplay(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<int>(id => displayList.Remove(displayList.Single(listItem => listItem.DisplayId == id)));

            //Action and Result
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _deleteDisplay.Execute(testDisplay));
        }

        [TestMethod]
        public async Task Display_Delete_Invalid_Display_No_Displays()
        {
            //Setup
            Display testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Test Display"
            };
            var displayList = new List<Display>();

            _displayRepository.Setup(a => a.DeleteDisplay(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<int>(id => displayList.Remove(displayList.Single(listItem => listItem.DisplayId == id)));

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _deleteDisplay.Execute(testDisplay));
        }
    }
}