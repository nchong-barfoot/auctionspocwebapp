using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayConfigurations
{
    /// <summary>
    /// Add DisplayConfiguration pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddDisplayConfigurationPipelineTests
    {
        private AddDisplayConfigurationPipeline _pipeline;
        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;
        private Mock<IDisplayRepository> _displayRepository;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<ILogger<DisplayConfiguration>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<DisplayConfiguration>>();
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _displayRepository = new Mock<IDisplayRepository>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _pipeline = new AddDisplayConfigurationPipeline(_logger.Object, _displayConfigurationRepository.Object, _displayRepository.Object, _displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Add_Pipeline_Valid_DisplayConfiguration()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0
            };
            var displayConfigurationList = new List<DisplayConfiguration>();

            _displayConfigurationRepository.Setup(a => a.AddDisplayConfiguration(It.IsAny<DisplayConfiguration>()))
                .Returns(Task.FromResult(testDisplayConfiguration))
                .Callback<DisplayConfiguration>(v => displayConfigurationList.Add(v));

            //Action
            var addedDisplayConfiguration = await _pipeline.Execute(testDisplayConfiguration);

            //Result
            Assert.IsFalse(addedDisplayConfiguration.IsCancelled);
            Assert.AreEqual(1, displayConfigurationList.Count);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Add_Pipeline_Invalid_DisplayConfiguration_No_Display_Exists()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0
            };
            //throw an invalid operation to simulate a failed database get for the display
            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>())).Throws(new InvalidOperationException());
            _displayConfigurationRepository.Setup(a => a.AddDisplayConfiguration(It.IsAny<DisplayConfiguration>()))
                .Returns(Task.FromResult(testDisplayConfiguration));

            //Action
            var addedDisplayConfiguration = await _pipeline.Execute(testDisplayConfiguration);

            //Result
            Assert.IsTrue(addedDisplayConfiguration.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Add_Pipeline_Invalid_DisplayConfiguration_Null_Values()
        {
            //Setup
            var displayConfigurationList = new List<DisplayConfiguration>();

            _displayConfigurationRepository.Setup(a => a.AddDisplayConfiguration(It.IsAny<DisplayConfiguration>()))
                .Returns(Task.FromResult((DisplayConfiguration) null));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(null));

            //Result
            Assert.IsTrue(displayConfigurationList.Count == 0);
        }
    }
}