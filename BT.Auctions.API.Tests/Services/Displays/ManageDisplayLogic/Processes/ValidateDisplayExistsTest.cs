using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Validate Display Exists Tests
    /// </summary>
    [TestClass]
    public class ValidateDisplayExistsTest
    {
        private ValidateDisplayExists _validateDisplayExists;
        private Mock<IDisplayRepository> _displayRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayRepository = new Mock<IDisplayRepository>();
            _validateDisplayExists = new ValidateDisplayExists(_displayRepository.Object);
        }

        [TestMethod]
        public async Task Display_Validate_Valid_Display_With_Display_Found_In_System()
        {
            //Setup
            var testDisplay = new Display()
            {
                DisplayId = 0,
                DisplayConfigurations = new List<DisplayConfiguration>
                {
                    new DisplayConfiguration
                    {
                        DisplayId = 0
                    }
                }
            };

            var tests = new List<Display>
            {
                new Display
                {
                    DisplayId = 0
                }
            };

            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
               .Returns<int>(displayConfigurationId => Task.FromResult(tests.Single(i => i.DisplayId == displayConfigurationId)));

            //Action
            await _validateDisplayExists.Execute(testDisplay);

            //Result
            Assert.IsFalse(testDisplay.IsCancelled);
        }

        [TestMethod]
        public async Task Display_Validate_Invalid_Display_With_Display_Not_Found()
        {
            //Setup
            var testDisplay = new Display()
            {
                DisplayId = 0,
                DisplayConfigurations = new List<DisplayConfiguration>
                {
                    new DisplayConfiguration
                    {
                        DisplayId = 0
                    }
                }
            };

            var tests = new List<Display>
            {
                new Display
                {
                    DisplayId = 1
                }
            };

            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
               .Returns<int>(displayConfigurationId => Task.FromResult(tests.Single(i => i.DisplayId == displayConfigurationId)));

            //Action
            await _validateDisplayExists.Execute(testDisplay);

            //Result
            Assert.IsTrue(testDisplay.IsCancelled);
        }
    }
}