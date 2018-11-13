using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Update DisplayConfiguration pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateDisplayConfigurationTest 
    {
        private UpdateDisplayConfiguration _updateDisplayConfiguration;
        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _updateDisplayConfiguration = new UpdateDisplayConfiguration(_displayConfigurationRepository.Object);
        }


        [TestMethod]
        public async Task DisplayConfiguration_Update_Valid_DisplayConfiguration()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0,
                IsActive = true
            };

            var displayConfigurationList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0,
                    IsActive = false
                }
            };

            _displayConfigurationRepository.Setup(a => a.UpdateDisplayConfiguration(It.IsAny<int>(), It.IsAny<DisplayConfiguration>()))
                .Returns((int id, DisplayConfiguration displayConfiguration) => Task.FromResult(displayConfiguration))
                .Callback<int, DisplayConfiguration>((i, v) => displayConfigurationList.Single(li => li.DisplayConfigurationId == v.DisplayConfigurationId).IsActive = v.IsActive);

            //Action
            await _updateDisplayConfiguration.Execute(testDisplayConfiguration);

            //Result
            Assert.IsTrue(displayConfigurationList.ElementAt(0).IsActive);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Update_Invalid_DisplayConfiguration()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 1
            };

            var displayConfigurationList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0
                }
            };

            _displayConfigurationRepository.Setup(a => a.UpdateDisplayConfiguration(It.IsAny<int>(), It.IsAny<DisplayConfiguration>()))
                .Returns((int id, DisplayConfiguration displayConfiguration) => Task.FromResult(displayConfiguration))
                .Callback<int, DisplayConfiguration>((i, v) => displayConfigurationList.Single(li => li.DisplayConfigurationId == v.DisplayConfigurationId).IsActive = v.IsActive);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateDisplayConfiguration.Execute(testDisplayConfiguration));
        }

        [TestMethod]
        public async Task DisplayConfiguration_Update_Invalid_DisplayConfiguration_No_DisplayConfigurations()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0
            };

            var displayConfigurationList = new List<DisplayConfiguration>();

            _displayConfigurationRepository.Setup(a => a.UpdateDisplayConfiguration(It.IsAny<int>(), It.IsAny<DisplayConfiguration>()))
                .Returns((int id, DisplayConfiguration displayConfiguration) => Task.FromResult(displayConfiguration))
                .Callback<int, DisplayConfiguration>((i, v) => displayConfigurationList.Single(li => li.DisplayConfigurationId == v.DisplayConfigurationId).IsActive = v.IsActive);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateDisplayConfiguration.Execute(testDisplayConfiguration));
        }

        [TestMethod]
        public async Task DisplayConfiguration_Update_No_Changes()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0
            };

            var displayConfigurationList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0
                }
            };

            _displayConfigurationRepository.Setup(a => a.UpdateDisplayConfiguration(It.IsAny<int>(), It.IsAny<DisplayConfiguration>()))
                .Returns((int id, DisplayConfiguration displayConfiguration) => Task.FromResult(displayConfiguration))
                .Callback<int, DisplayConfiguration>((i, v) => displayConfigurationList.Single(li => li.DisplayConfigurationId == v.DisplayConfigurationId).IsActive = v.IsActive);

            //Action
            await _updateDisplayConfiguration.Execute(testDisplayConfiguration);

            //Result
            Assert.IsFalse(displayConfigurationList.ElementAt(0).IsActive);
        }
    }
}