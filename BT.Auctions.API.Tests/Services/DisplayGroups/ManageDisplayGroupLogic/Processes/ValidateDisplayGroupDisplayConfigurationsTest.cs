using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Validate DisplayGroup Display Configurations Exists Tests
    /// </summary>
    [TestClass]
    public class ValidateDisplayGroupDisplayConfigurationsTest
    {
        private ValidateDisplayGroupDisplayConfigurations _validateDisplayGroupDisplayConfigurationsExist;

        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _validateDisplayGroupDisplayConfigurationsExist = new ValidateDisplayGroupDisplayConfigurations(_displayConfigurationRepository.Object);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Valid_DisplayGroup_With_DisplayConfiguration_Found_Not_Allocated_To_Another_Group()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    new DisplayGroupConfigurations
                    {
                        DisplayConfigurationId = 0
                    }
                }
            };

            var testConfigurations = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0,
                    Display = new Display
                    {
                        DisplayId = 0
                    }
                }
            };

            _displayConfigurationRepository.Setup(a => a.GetDisplayConfigurationById(It.IsAny<int>()))
               .Returns<int>(displayConfigurationId => Task.FromResult(testConfigurations.Single(i => i.DisplayConfigurationId == displayConfigurationId)));

            //Action
            await _validateDisplayGroupDisplayConfigurationsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsFalse(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_With_DisplayConfiguration_Found_Allocated_To_Another_Group()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    new DisplayGroupConfigurations
                    {
                        DisplayConfigurationId = 0
                    }
                }
            };

            var testSession = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0
                }
            };

            _displayConfigurationRepository.Setup(a => a.GetDisplayConfigurationById(It.IsAny<int>()))
               .Returns<int>(displayConfigurationId => Task.FromResult(testSession.Single(i => i.DisplayConfigurationId == displayConfigurationId)));

            //Action
            await _validateDisplayGroupDisplayConfigurationsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_With_DisplayConfiguration_Not_Found()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    new DisplayGroupConfigurations
                    {
                        DisplayConfigurationId = 0
                    }
                }
            };

            var testConfigurations = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 1
                }
            };

            _displayConfigurationRepository.Setup(a => a.GetDisplayConfigurationById(It.IsAny<int>()))
              .Returns<int>(displayConfigurationId => Task.FromResult(testConfigurations.Single(i => i.DisplayConfigurationId == displayConfigurationId)));

            //Action
            await _validateDisplayGroupDisplayConfigurationsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }
    }
}