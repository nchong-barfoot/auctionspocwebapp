using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Add displayConfiguration pipeline process tests
    /// </summary>
    [TestClass]
    public class AddDisplayConfigurationTests
    {
        private AddDisplayConfiguration _addDisplayConfiguration;
        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _addDisplayConfiguration = new AddDisplayConfiguration(_displayConfigurationRepository.Object);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Add_Valid_DisplayConfiguration_To_DisplayConfiguration_List()
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
            await _addDisplayConfiguration.Execute(testDisplayConfiguration);

            //Result
            Assert.AreEqual(displayConfigurationList.Count, 1);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Add_Invalid_DisplayConfiguration()
        {
            //Setup
            DisplayConfiguration testDisplayConfiguration = null;
            var displayConfigurationList = new List<DisplayConfiguration>();

            _displayConfigurationRepository.Setup(a => a.AddDisplayConfiguration(It.IsAny<DisplayConfiguration>()))
                .Returns(Task.FromResult(testDisplayConfiguration))
                .Callback<DisplayConfiguration>(v => displayConfigurationList.Add(v));

            //Action
            await _addDisplayConfiguration.Execute(testDisplayConfiguration);

            //Result
            Assert.AreEqual(displayConfigurationList.Count, 1);
        }
    }
}