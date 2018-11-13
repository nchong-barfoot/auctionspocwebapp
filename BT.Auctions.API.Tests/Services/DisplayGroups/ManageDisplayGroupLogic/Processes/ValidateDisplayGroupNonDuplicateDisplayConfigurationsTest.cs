using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Validate Display Groups have unique display configurations
    /// </summary>
    [TestClass]
    public class ValidateDisplayGroupNonDuplicateDisplayConfigurationsTest
    {
        private ValidateDisplayGroupNonDuplicateDisplayConfigurations _validateDisplayGroupNonDuplicateDisplayConfigurations;

        [TestInitialize]
        public void Initialize()
        {
            _validateDisplayGroupNonDuplicateDisplayConfigurations = new ValidateDisplayGroupNonDuplicateDisplayConfigurations();
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Valid_DisplayGroup_With_Non_Duplicate_Display_Group_Configurations_Found_In_System()
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

            //Action
            await _validateDisplayGroupNonDuplicateDisplayConfigurations.Execute(testDisplayGroup);

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
                    },
                    new DisplayGroupConfigurations
                    {
                        DisplayGroupId = 0
                    }
                }
            };

            //Action
            await _validateDisplayGroupNonDuplicateDisplayConfigurations.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }
    }
}