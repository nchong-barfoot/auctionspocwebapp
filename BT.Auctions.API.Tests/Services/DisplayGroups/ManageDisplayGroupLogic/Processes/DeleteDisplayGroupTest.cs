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
    /// Delete DisplayGroup pipeline process test
    /// </summary>
    [TestClass]
    public class DeleteDisplayGroupTest
    {
        private DeleteDisplayGroup _deleteDisplayGroup;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _deleteDisplayGroup = new DeleteDisplayGroup(_displayGroupRepository.Object);
        }


        [TestMethod]
        public async Task DisplayGroup_Delete_Valid_DisplayGroup()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                Name = "Test DisplayGroup"
            };

            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0,
                    Name = "Test DisplayGroup"
                }
            };

            _displayGroupRepository.Setup(a => a.DeleteDisplayGroup(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplayGroup))
                .Callback<int>(id => displayGroupList.Remove(displayGroupList.Single(listItem => listItem.DisplayGroupId == id)));

            //Action
            await _deleteDisplayGroup.Execute(testDisplayGroup);

            //Result
            Assert.AreEqual(displayGroupList.Count, 0);
        }

        [TestMethod]
        public async Task DisplayGroup_Delete_Invalid_DisplayGroup()
        {
            //Setup
            DisplayGroup testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                Name = "Test DisplayGroup"
            };
            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 1,
                    Name = "Test DisplayGroup Stays Here"
                }
            };

            _displayGroupRepository.Setup(a => a.DeleteDisplayGroup(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplayGroup))
                .Callback<int>(id => displayGroupList.Remove(displayGroupList.Single(listItem => listItem.DisplayGroupId == id)));

            //Action and Result
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _deleteDisplayGroup.Execute(testDisplayGroup));
        }

        [TestMethod]
        public async Task DisplayGroup_Delete_Invalid_DisplayGroup_No_DisplayGroups()
        {
            //Setup
            DisplayGroup testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                Name = "Test DisplayGroup"
            };
            var displayGroupList = new List<DisplayGroup>();

            _displayGroupRepository.Setup(a => a.DeleteDisplayGroup(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplayGroup))
                .Callback<int>(id => displayGroupList.Remove(displayGroupList.Single(listItem => listItem.DisplayGroupId == id)));

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _deleteDisplayGroup.Execute(testDisplayGroup));
        }
    }
}