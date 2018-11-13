using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Used to test the Display  has a name
    /// </summary>
    [TestClass]
    public class ValidateDisplayHasNameTest
    {
        private ValidateDisplayHasName _validateDisplayHasName;

        [TestInitialize]
        public void Initialize()
        {
            _validateDisplayHasName = new ValidateDisplayHasName();
        }

        [TestMethod]
        public async Task DisplayHas_Validate_Valid_DisplayHas_With_Name()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Test"
            };

            //Action
            await _validateDisplayHasName.Execute(testDisplay);

            //Result
            Assert.IsFalse(testDisplay.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayHas_Validate_Invalid_DisplayHas_Incomplete_Name()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0
            };

            //Action
            await _validateDisplayHasName.Execute(testDisplay);

            //Result
            Assert.IsTrue(testDisplay.IsCancelled);
        }
    }
}