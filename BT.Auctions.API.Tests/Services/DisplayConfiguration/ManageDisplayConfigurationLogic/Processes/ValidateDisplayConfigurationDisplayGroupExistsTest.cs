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
    /// Validate DisplayConfiguration DisplayGroup Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateDisplayConfigurationDisplayGroupExistsTest
    {
        private ValidateDisplayConfigurationDisplayGroupExists _validateDisplayConfigurationDisplayGroupExists;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _validateDisplayConfigurationDisplayGroupExists = new ValidateDisplayConfigurationDisplayGroupExists(_displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Validate_Valid_DisplayConfiguration_With_DisplayGroup_Found()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration()
            {
                DisplayConfigurationId = 0,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    new DisplayGroupConfigurations
                    {
                        DisplayGroupId  = 0
                    }
                }
            };

            var testSession = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0
                }
            };

            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.DisplayGroupId == auctionSessionId)));

            //Action
            await _validateDisplayConfigurationDisplayGroupExists.Execute(testDisplayConfiguration);

            //Result
            Assert.IsFalse(testDisplayConfiguration.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayConfiguration_Validate_Valid_DisplayConfiguration_Without_DisplayGroup_Found()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration()
            {
                DisplayConfigurationId = 0,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    new DisplayGroupConfigurations
                    {
                        DisplayGroupId  = 0
                    }
                }
            };

            var testDisplayGroup = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 1
                }
            };

            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>()))
                .Returns<int>(displayGroupId => Task.FromResult(testDisplayGroup.Single(i => i.DisplayGroupId == displayGroupId)));

            //Action
            await _validateDisplayConfigurationDisplayGroupExists.Execute(testDisplayConfiguration);

            //Result
            Assert.IsTrue(testDisplayConfiguration.IsCancelled);
        }
    }
}