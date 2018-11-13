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
    /// Delete DisplayConfiguration pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class DeleteDisplayConfigurationPipelineTests
    {
        private DeleteDisplayConfigurationPipeline _pipeline;
        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;
        private Mock<ILogger<DisplayConfiguration>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<DisplayConfiguration>>();
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _pipeline = new DeleteDisplayConfigurationPipeline(_logger.Object, _displayConfigurationRepository.Object);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Delete_Pipeline_Valid_DisplayConfiguration()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0,
                DisplayId = 1
            };
            var displayConfigurationList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0,
                    DisplayId = 1
                }
            };

            _displayConfigurationRepository.Setup(a => a.GetDisplayConfigurationById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplayConfiguration));
            _displayConfigurationRepository.Setup(a => a.DeleteDisplayConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplayConfiguration))
                .Callback<int>(id => displayConfigurationList.Remove(displayConfigurationList.Single(listItem => listItem.DisplayConfigurationId == id)));


            //Action
            await _pipeline.Execute(testDisplayConfiguration);

            //Result
            Assert.AreEqual(displayConfigurationList.Count, 0);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Delete_Pipeline_Invalid_DisplayConfiguration()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0,
                DisplayId = 1
            };
            var displayConfigurationList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 1,
                    DisplayId = 1
                }
            };

            _displayConfigurationRepository.Setup(a => a.GetDisplayConfigurationById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(displayConfigurationList.Single(d => d.DisplayConfigurationId == id)));

            //Action
            await _pipeline.Execute(testDisplayConfiguration);

            //Result
            Assert.IsTrue(testDisplayConfiguration.IsCancelled);
        }
    }
}
