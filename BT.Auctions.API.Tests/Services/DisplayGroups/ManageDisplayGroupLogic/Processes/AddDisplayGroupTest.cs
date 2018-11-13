using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Add displayGroup pipeline process tests
    /// </summary>
    [TestClass]
    public class AddDisplayGroupTests
    {
        private AddDisplayGroup _addDisplayGroup;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _addDisplayGroup = new AddDisplayGroup(_displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task DisplayGroup_Add_Valid_DisplayGroup_To_DisplayGroup_List()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0
            };
            var displayGroupList = new List<DisplayGroup>();
            _displayGroupRepository.Setup(a => a.AddDisplayGroup(It.IsAny<DisplayGroup>()))
                .Returns(Task.FromResult(testDisplayGroup))
                .Callback<DisplayGroup>(v => displayGroupList.Add(v));

            //Action
            await _addDisplayGroup.Execute(testDisplayGroup);

            //Result
            Assert.AreEqual(displayGroupList.Count, 1);
        }

        [TestMethod]
        public async Task DisplayGroup_Add_Invalid_DisplayGroup()
        {
            //Setup
            DisplayGroup testDisplayGroup = null;
            var displayGroupList = new List<DisplayGroup>();

            _displayGroupRepository.Setup(a => a.AddDisplayGroup(It.IsAny<DisplayGroup>()))
                .Returns(Task.FromResult(testDisplayGroup))
                .Callback<DisplayGroup>(v => displayGroupList.Add(v));

            //Action
            await _addDisplayGroup.Execute(testDisplayGroup);

            //Result
            Assert.AreEqual(displayGroupList.Count, 1);
        }
    }
}