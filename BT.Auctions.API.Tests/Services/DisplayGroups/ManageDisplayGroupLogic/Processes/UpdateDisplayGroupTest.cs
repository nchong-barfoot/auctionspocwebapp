using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Update DisplayGroup pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateDisplayGroupTest 
    {
        private UpdateDisplayGroup _updateDisplayGroup;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _updateDisplayGroup = new UpdateDisplayGroup(_displayGroupRepository.Object);
        }


        [TestMethod]
        public async Task DisplayGroup_Update_Valid_DisplayGroup()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                Name = "Updated Test Display Group"
            };

            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0,
                    Name = "Test Display Group"
                }
            };

            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()))
                .Returns((int id, DisplayGroup displayGroup) => Task.FromResult(displayGroup))
                .Callback<int, DisplayGroup>((i, v) => displayGroupList.Single(li => li.DisplayGroupId == v.DisplayGroupId).Name = v.Name);

            //Action
            await _updateDisplayGroup.Execute(testDisplayGroup);

            //Result
            Assert.AreEqual(displayGroupList.ElementAt(0).Name, "Updated Test Display Group");
        }

        [TestMethod]
        public async Task DisplayGroup_Update_Invalid_DisplayGroup()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 1
            };

            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0
                }
            };

            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()))
                .Returns((int id, DisplayGroup displayGroup) => Task.FromResult(displayGroup))
                .Callback<int, DisplayGroup>((i, v) => displayGroupList.Single(li => li.DisplayGroupId == v.DisplayGroupId).Name = v.Name);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateDisplayGroup.Execute(testDisplayGroup));
        }

        [TestMethod]
        public async Task DisplayGroup_Update_Invalid_DisplayGroup_No_DisplayGroups()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0
            };

            var displayGroupList = new List<DisplayGroup>();

            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()))
                .Returns((int id, DisplayGroup displayGroup) => Task.FromResult(displayGroup))
                .Callback<int, DisplayGroup>((i, v) => displayGroupList.Single(li => li.DisplayGroupId == v.DisplayGroupId).Name = v.Name);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateDisplayGroup.Execute(testDisplayGroup));
        }

        [TestMethod]
        public async Task DisplayGroup_Update_No_Changes()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                Name = "Updated Test Display Group"
            };

            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0,
                    Name = "Previous Display Group Name"
                }
            };

            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()))
                .Returns((int id, DisplayGroup displayGroup) => Task.FromResult(displayGroup))
                .Callback<int, DisplayGroup>((i, v) => displayGroupList.Single(li => li.DisplayGroupId == v.DisplayGroupId).Name = v.Name);

            //Action
            await _updateDisplayGroup.Execute(testDisplayGroup);

            //Result
            Assert.AreEqual(displayGroupList.ElementAt(0).Name, "Updated Test Display Group");
        }
    }
}