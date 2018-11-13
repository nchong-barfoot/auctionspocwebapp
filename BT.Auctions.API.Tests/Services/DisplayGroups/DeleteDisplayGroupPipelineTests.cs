using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups
{
    /// <summary>
    /// Delete DisplayGroup pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class DeleteDisplayGroupPipelineTests
    {
        private DeleteDisplayGroupPipeline _pipeline;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<ILogger<DisplayGroup>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<DisplayGroup>>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _pipeline = new DeleteDisplayGroupPipeline(_logger.Object, _displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task DisplayGroup_Delete_Pipeline_Valid_DisplayGroup()
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
                    Name = "Test DisplayGroup to remove"
                }
            };

            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplayGroup));
            _displayGroupRepository.Setup(a => a.DeleteDisplayGroup(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplayGroup))
                .Callback<int>(id => displayGroupList.Remove(displayGroupList.Single(listItem => listItem.DisplayGroupId == id)));


            //Action
            await _pipeline.Execute(testDisplayGroup);

            //Result
            Assert.AreEqual(displayGroupList.Count, 0);
        }

        [TestMethod]
        public async Task DisplayGroup_Delete_Pipeline_Invalid_DisplayGroup()
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
                    DisplayGroupId = 1,
                    Name = "Test DisplayGroup to stay"
                }
            };

            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>())).Throws(new InvalidOperationException());

            //Action
            await _pipeline.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }
    }
}
