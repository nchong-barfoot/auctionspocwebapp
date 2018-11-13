using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services
{
    [TestClass]
    public class DisplayConfigurationsServiceTests
    {
        private DisplayConfigurationService _displayConfigurationsService;
        private Mock<ILogger<DisplayConfiguration>> _logger;
        private Mock<IDisplayConfigurationRepository> _displayConfigurationRepository;
        private Mock<IDisplayRepository> _displayRepository;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<IVenueRepository> _venueRepository;
        private List<DisplayConfiguration> _displayConfigurationList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<DisplayConfiguration>>();
            _displayConfigurationRepository = new Mock<IDisplayConfigurationRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _displayRepository = new Mock<IDisplayRepository>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _displayConfigurationList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0,
                    DisplayMode = DisplayMode.OrderOfSale,
                    IsActive = true,
                    PlayAudio = false,
                    PlayVideo = true
                },
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 1,
                    DisplayMode = DisplayMode.Placeholder,
                    IsActive = true,
                    PlayAudio = false,
                    PlayVideo = true
                }
            };

            _displayConfigurationsService = new DisplayConfigurationService(_logger.Object, _displayConfigurationRepository.Object, _displayRepository.Object, _venueRepository.Object, _displayGroupRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public async Task Update_DisplayConfiguration_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var displayConfigurationDto = new DisplayConfigurationDto {IsActive = false};
            _displayConfigurationRepository.Setup(v => v.GetDisplayConfigurationById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_displayConfigurationList.Single(v => v.DisplayConfigurationId == id)));
            _displayConfigurationRepository.Setup(a => a.UpdateDisplayConfiguration(It.IsAny<int>(), It.IsAny<DisplayConfiguration>()))
                .Returns((int id, DisplayConfiguration displayConfiguration) => Task.FromResult(displayConfiguration))
                .Callback<int, DisplayConfiguration>((i, v) => _displayConfigurationList.Single(li => li.DisplayConfigurationId == v.DisplayConfigurationId).IsActive = v.IsActive);

            //Action
            var displayConfigurationResult = await _displayConfigurationsService.UpdateDisplayConfiguration(1, displayConfigurationDto);

            //Result
            Assert.IsFalse(displayConfigurationResult.IsActive);
            Assert.IsFalse(displayConfigurationResult.IsCancelled);
            
        }

        [TestMethod]
        public async Task Update_DisplayConfiguration_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var displayConfigurationDto = new DisplayConfigurationDto { IsActive = false };
            _displayConfigurationRepository.Setup(v => v.GetDisplayConfigurationById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_displayConfigurationList.Single(v => v.DisplayConfigurationId == id)));
            _displayConfigurationRepository.Setup(a => a.UpdateDisplayConfiguration(It.IsAny<int>(), It.IsAny<DisplayConfiguration>()));

            //Action
            var displayConfigurationResult = await _displayConfigurationsService.UpdateDisplayConfiguration(2, displayConfigurationDto);

            //Result
            Assert.IsTrue(displayConfigurationResult.IsCancelled);
        }
        
        [TestMethod]
        public async Task Add_Invalid_DisplayConfiguration_To_System_No_DisplayConfiguration_Details_Provided()
        {
            //Setup
            var displayConfigurationToAddDto = new DisplayConfigurationDto
            {

            };

            _displayConfigurationRepository.Setup(a => a.AddDisplayConfiguration(It.IsAny<DisplayConfiguration>()));

            //Action
            var displayConfigurationAdded = await _displayConfigurationsService.AddDisplayConfiguration(displayConfigurationToAddDto);

            //Result
            Assert.IsTrue(displayConfigurationAdded.IsCancelled);
            Assert.AreEqual(2, _displayConfigurationList.Count);
        }

        [TestMethod]
        public async Task Add_Valid_DisplayConfiguration_To_System_With_DisplayConfiguration_Details_Provided()
        {
            //Setup
            var displayConfigurationToAddDto = new DisplayConfigurationDto
            {
                DisplayMode = DisplayMode.PropertyDetails,
                DisplayId = 0,
                IsActive = true
            };
            var testVenue = new Venue
            {
                VenueId = 3
            };

            _venueRepository.Setup(a => a.GetVenueById(It.IsAny<int>())).Returns(Task.FromResult(testVenue));
            _displayConfigurationRepository.Setup(a => a.AddDisplayConfiguration(It.IsAny<DisplayConfiguration>()))
                .Returns(Task.FromResult(new DisplayConfiguration()));

            //Action
            var displayConfigurationAdded = await _displayConfigurationsService.AddDisplayConfiguration(displayConfigurationToAddDto);

            //Result
            Assert.IsFalse(displayConfigurationAdded.IsCancelled);
        }

        [TestMethod]
        public async Task Get_DisplayConfigurations()
        {
            //Setup
            var displayConfigurationsList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayId = 1,
                    DisplayConfigurationId = 0
                }
            };

            _displayConfigurationRepository.Setup(v => v.GetDisplayConfigurations())
                .Returns(Task.FromResult(displayConfigurationsList.AsEnumerable()));

            //Action
            var testConfigurations = await _displayConfigurationsService.GetDisplayConfigurations();

            //Result
            Assert.AreEqual(1, testConfigurations.Count());
        }

        [TestMethod]
        public async Task Get_DisplayConfigurations_By_DisplayGroupId()
        {
            //Setup
            var displayConfigurationsList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayId = 1,
                    DisplayConfigurationId = 0,
                    DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                    {
                        new DisplayGroupConfigurations
                        {
                            DisplayGroupId = 1
                        }
                    }
                }
            };

            _displayConfigurationRepository.Setup(v => v.GetDisplayGroupConfigurationsByDisplayGroupId(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(displayConfigurationsList.Where(d => d.DisplayGroupConfigurations.Any(g => g.DisplayGroupId == id))));

            //Action
            var testGroupConfigurations = await _displayConfigurationsService.GetDisplayGroupConfigurationsByDisplayGroupId(1);

            //Result
            Assert.AreEqual(1, testGroupConfigurations.Count());
        }

        [TestMethod]
        public async Task DisplayConfiguration_Delete_Valid_Configuration()
        {
            //Setup
            var testDisplayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 0,
                DisplayId = 1
            };
            var displayConfigurationList = new List<DisplayConfiguration>
            {
                new DisplayConfiguration
                {
                    DisplayConfigurationId = 0,
                    DisplayId = 1
                }
            };

            _displayConfigurationRepository.Setup(a => a.GetDisplayConfigurationById(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplayConfiguration));
            _displayConfigurationRepository.Setup(a => a.DeleteDisplayConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplayConfiguration))
                .Callback<int>(id => displayConfigurationList.Remove(displayConfigurationList.Single(listItem => listItem.DisplayConfigurationId == id)));


            //Action
            await _displayConfigurationsService.DeleteDisplayConfiguration(0);

            //Result
            Assert.AreEqual(displayConfigurationList.Count, 0);
        }
    }
}
