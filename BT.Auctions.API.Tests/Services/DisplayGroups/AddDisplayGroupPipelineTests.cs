using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.DisplayGroups
{
    /// <summary>
    /// Add DisplayGroup pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddDisplayGroupPipelineTests
    {
        private AddDisplayGroupPipeline _pipeline;
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
            _pipeline = new AddDisplayGroupPipeline(_logger.Object, _displayGroupRepository.Object, _displayConfigurationRepository.Object, _auctionSessionRepository.Object, _venueRepository.Object);
        }

        [TestMethod]
        public async Task DisplayGroup_Add_Pipeline_Valid_DisplayGroup()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                Name = "Display Group"
            };
            var displayGroupList = new List<DisplayGroup>();

            _displayGroupRepository.Setup(a => a.AddDisplayGroup(It.IsAny<DisplayGroup>()))
                .Returns(Task.FromResult(testDisplayGroup))
                .Callback<DisplayGroup>(v => displayGroupList.Add(v));

            //Action
            var addedDisplayGroup = await _pipeline.Execute(testDisplayGroup);

            //Result
            Assert.IsFalse(addedDisplayGroup.IsCancelled);
            Assert.AreEqual(1, displayGroupList.Count);
        }

        [TestMethod]
        public async Task DisplayGroup_Add_Pipeline_Invalid_DisplayGroup_No_Name()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 0
            };

            //Action
            var addedDisplayGroup = await _pipeline.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(addedDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Add_Pipeline_Invalid_DisplayGroup_Null_Values()
        {
            //Setup
            var displayGroupList = new List<DisplayGroup>();

            _displayGroupRepository.Setup(a => a.AddDisplayGroup(It.IsAny<DisplayGroup>()))
                .Returns(Task.FromResult((DisplayGroup) null));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(null));

            //Result
            Assert.IsTrue(displayGroupList.Count == 0);
        }

        [TestMethod]
        public async Task DisplayGroup_Add_Pipeline_Invalid_DisplayGroup_No_Venue()
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

            _displayGroupRepository.Setup(a => a.AddDisplayGroup(It.IsAny<DisplayGroup>()));
            _venueRepository.Setup(v => v.GetVenueById(It.IsAny<int>())).Throws(new InvalidOperationException());
            
            //Action
            var updatedDisplayConfiguration = await _pipeline.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(updatedDisplayConfiguration.IsCancelled);
        }
    }
}