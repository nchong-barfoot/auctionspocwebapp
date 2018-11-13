using System;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups
{
    /// <summary>
    /// Update DisplayGroup pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateDisplayGroupPipelineTests
    {
        private UpdateDisplayGroupPipeline _pipeline;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<ILogger<DisplayGroup>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<DisplayGroup>>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _pipeline = new UpdateDisplayGroupPipeline(_logger.Object, _displayGroupRepository.Object, _displayConfigurationRepository.Object, _auctionSessionRepository.Object, _venueRepository.Object);
        }

        [TestMethod]
        public async Task DisplayGroup_Update_Pipeline_Valid_DisplayGroup()
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
                    Name = "Previous Test Display Group"
                }
            };

            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()))
                .Returns((int id, DisplayGroup displayGroup) => Task.FromResult(displayGroup))
                .Callback<int, DisplayGroup>((i, v) => displayGroupList.Single(li => li.DisplayGroupId == v.DisplayGroupId).Name = v.Name);
            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplayGroup));


            //Action
            var updatedDisplayGroup = await _pipeline.Execute(testDisplayGroup);

            //Result
            Assert.IsFalse(updatedDisplayGroup.IsCancelled);
            Assert.AreEqual(displayGroupList[0].Name, "Updated Test Display Group");
        }

        [TestMethod]
        public async Task DisplayGroup_Update_Pipeline_Invalid_DisplayGroup_No_Venue()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,

            };
            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0
                }
            };

            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()));
            _displayGroupRepository.Setup(a => a.GetDisplayGroupById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplayGroup));
            

            //Action
            var updatedDisplayGroup = await _pipeline.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(updatedDisplayGroup.IsCancelled);
        }
    }
}
