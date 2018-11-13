using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayConfigurations
{
    /// <summary>
    /// Update DisplayConfiguration pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateDisplayConfigurationPipelineTests
    {
        private UpdateDisplayConfigurationPipeline _pipeline;
        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;
        private Mock<IDisplayRepository> _displayRepository;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<ILogger<DisplayConfiguration>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<DisplayConfiguration>>();
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _displayRepository = new Mock<IDisplayRepository>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _pipeline = new UpdateDisplayConfigurationPipeline(_logger.Object, _displayConfigurationRepository.Object, _displayRepository.Object, _displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Update_Pipeline_Valid_DisplayConfiguration()
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
            _displayConfigurationRepository.Setup(a => a.GetDisplayConfigurationById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplayConfiguration));


            //Action
            var updatedDisplayConfiguration = await _pipeline.Execute(testDisplayConfiguration);

            //Result
            Assert.IsFalse(updatedDisplayConfiguration.IsCancelled);
            Assert.IsTrue(displayConfigurationList[0].IsActive);
        }
    }
}
