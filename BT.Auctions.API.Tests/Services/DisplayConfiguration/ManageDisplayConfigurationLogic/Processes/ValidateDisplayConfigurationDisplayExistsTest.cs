using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Validate DisplayConfiguration Display Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateDisplayConfigurationDisplayExistsTest
    {
        private ValidateDisplayConfigurationDisplayExists _validateDisplayConfigurationDisplayExists;
        private Mock<IDisplayRepository> _displayRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayRepository = new Mock<IDisplayRepository>();
            _validateDisplayConfigurationDisplayExists = new ValidateDisplayConfigurationDisplayExists(_displayRepository.Object);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Validate_Valid_DisplayConfiguration_With_Display_Found()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration()
            {
                DisplayId = 0
            };

            var testSession = new List<Display>
            {
                new Display
                {
                    DisplayId = 0
                }
            };

            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.DisplayId == auctionSessionId)));

            //Action
            await _validateDisplayConfigurationDisplayExists.Execute(testDisplayConfiguration);

            //Result
            Assert.IsFalse(testDisplayConfiguration.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Validate_Valid_DisplayConfiguration_Without_Display_Found()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration()
            {
                DisplayId = 0
            };

            var testDisplay = new List<Display>
            {
                new Display
                {
                    DisplayId = 1
                }
            };

            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
                .Returns<int>(displayId => Task.FromResult(testDisplay.Single(i => i.DisplayId == displayId)));

            //Action
            await _validateDisplayConfigurationDisplayExists.Execute(testDisplayConfiguration);

            //Result
            Assert.IsTrue(testDisplayConfiguration.IsCancelled);
        }
    }
}