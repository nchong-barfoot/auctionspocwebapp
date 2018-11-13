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
    /// Validate DisplayGroup Exists Tests
    /// </summary>
    [TestClass]
    public class ValidateDisplayGroupExistsTest
    {
        private ValidateDisplayGroupExists _validateDisplayGroupExists;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _validateDisplayGroupExists = new ValidateDisplayGroupExists(_displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Valid_DisplayGroup_With_DisplayGroup_Found_In_System()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    new DisplayGroupConfigurations
                    {
                        DisplayGroupId = 0
                    }
                }
            };

            var testGroups = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0
                }
            };

            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>()))
               .Returns<int>(displayConfigurationId => Task.FromResult(testGroups.Single(i => i.DisplayGroupId == displayConfigurationId)));

            //Action
            await _validateDisplayGroupExists.Execute(testDisplayGroup);

            //Result
            Assert.IsFalse(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_With_DisplayGroup_Not_Found()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    new DisplayGroupConfigurations
                    {
                        DisplayGroupId = 0
                    }
                }
            };

            var testGroups = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 1
                }
            };

            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>()))
               .Returns<int>(displayConfigurationId => Task.FromResult(testGroups.Single(i => i.DisplayGroupId == displayConfigurationId)));

            //Action
            await _validateDisplayGroupExists.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }
    }
}