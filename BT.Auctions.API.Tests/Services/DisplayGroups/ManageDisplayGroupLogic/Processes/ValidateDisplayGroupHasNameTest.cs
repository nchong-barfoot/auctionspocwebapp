using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Used to test the Display Group has a name
    /// </summary>
    [TestClass]
    public class ValidateDisplayGroupHasNameTest
    {
        private ValidateDisplayGroupHasName _validateDisplayGroupHasName;

        [TestInitialize]
        public void Initialize()
        {
            _validateDisplayGroupHasName = new ValidateDisplayGroupHasName();
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Valid_DisplayGroup_With_Name()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                Name = "Test"
            };

            //Action
            await _validateDisplayGroupHasName.Execute(testDisplayGroup);

            //Result
            Assert.IsFalse(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_Has_Incomplete_Name()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0
            };

            //Action
            await _validateDisplayGroupHasName.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }
    }
}