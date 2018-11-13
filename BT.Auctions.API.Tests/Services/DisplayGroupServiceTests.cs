using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services
{
    [TestClass]
    public class DisplayGroupsServiceTests
    {
        private DisplayGroupService _displayGroupsService;
        private Mock<ILogger<DisplayGroup>> _logger;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;
        private Mock<IDisplayConfigurationRepository> _displayRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<IVenueRepository> _venueRepository;
        private List<DisplayGroup> _displayGroupList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<DisplayGroup>>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _displayRepository = new Mock<IDisplayConfigurationRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 0,
                    Name = "Display Group 1"
                },
                new DisplayGroup
                {
                    DisplayGroupId = 1,
                    Name = "Display Group 2"
                }
            };

            _displayGroupsService = new DisplayGroupService(_logger.Object, _displayGroupRepository.Object, _displayRepository.Object, _auctionSessionRepository.Object, _venueRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public async Task Get_DisplayGroup_By_Auction_Session_Id_With_Valid_DisplayGroup_Requested()
        {
            //Setup
            _displayGroupRepository.Setup(v => v.GetDisplayGroups())
                .Returns(Task.FromResult(_displayGroupList.AsEnumerable()));

            //Action
            var testDisplayGroups = await _displayGroupsService.GetDisplayGroups();

            //Result
            Assert.AreEqual(2, testDisplayGroups.Count());
        }

        [TestMethod]
        public async Task Update_DisplayGroup_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var displayGroupDto = new DisplayGroupDto { Name = "TestGroup"};

            _displayGroupRepository.Setup(v => v.GetDisplayGroupById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_displayGroupList.Single(v => v.DisplayGroupId == id)));

            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()))
                .Returns((int id, DisplayGroup displayGroup) => Task.FromResult(displayGroup))
                .Callback<int, DisplayGroup>((i, v) => _displayGroupList.Single(li => li.DisplayGroupId == v.DisplayGroupId).Name = v.Name);

            //Action
            var displayGroupResult = await _displayGroupsService.UpdateDisplayGroup(1, displayGroupDto);

            //Result
            Assert.AreEqual("TestGroup", displayGroupResult.Name);
            Assert.IsFalse(displayGroupResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_DisplayGroup_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var displayGroupDto = new DisplayGroupDto { Name = "Test Display Group" };
            _displayGroupRepository.Setup(v => v.GetDisplayGroupById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_displayGroupList.Single(v => v.DisplayGroupId == id)));
            _displayGroupRepository.Setup(a => a.UpdateDisplayGroup(It.IsAny<int>(), It.IsAny<DisplayGroup>()));

            //Action
            var displayGroupResult = await _displayGroupsService.UpdateDisplayGroup(2, displayGroupDto);

            //Result
            Assert.IsTrue(displayGroupResult.IsCancelled);
        }
        
        [TestMethod]
        public async Task Add_Invalid_DisplayGroup_To_System_No_DisplayGroup_Details_Provided()
        {
            //Setup
            var displayGroupToAddDto = new DisplayGroupDto
            {

            };

            _displayGroupRepository.Setup(a => a.AddDisplayGroup(It.IsAny<DisplayGroup>()));

            //Action
            var displayGroupAdded = await _displayGroupsService.AddDisplayGroup(displayGroupToAddDto);

            //Result
            Assert.IsTrue(displayGroupAdded.IsCancelled);
            Assert.AreEqual(2, _displayGroupList.Count);
        }

        [TestMethod]
        public async Task Add_Valid_DisplayGroup_To_System_With_DisplayGroup_Details_Provided()
        {
            //Setup
            var displayGroupToAddDto = new DisplayGroupDto
            {
                 Name = "Test Display Group"
            };

            var auctionSessions = new List<AuctionSession>
            {
                new AuctionSession
                {
                    Name = "Session",
                    AuctionSessionId = 0
                }
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionsByDateTime(It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(Task.FromResult(auctionSessions.AsEnumerable()));
            _displayGroupRepository.Setup(a => a.AddDisplayGroup(It.IsAny<DisplayGroup>()))
                .Returns(Task.FromResult(new DisplayGroup()));

            //Action
            var displayGroupAdded = await _displayGroupsService.AddDisplayGroup(displayGroupToAddDto);

            //Result
            Assert.IsFalse(displayGroupAdded.IsCancelled);
        }

        [TestMethod]
        public async Task Get_PagedDisplayGroups_With_Valid_Results()
        {
            //Setup
            int pageNumber = 1;
            int pageSize = 3;
            int? auctionSessionId = 1;
            int? venueId = null;
            string timeZone = "New Zealand Standard Time";

            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 1,
                    Name = "Test DisplayGroup 1"
                },
                new DisplayGroup
                {
                    DisplayGroupId = 2,
                    Name = "Test DisplayGroup 2"
                },
                new DisplayGroup
                {
                    DisplayGroupId = 3,
                    Name = "Test DisplayGroup 3"
                },
                new DisplayGroup
                {
                    DisplayGroupId = 4,
                    Name = "Test DisplayGroup 4"
                },
                new DisplayGroup
                {
                    DisplayGroupId = 5,
                    Name = "Test DisplayGroup 5"
                }
            }.AsQueryable();

            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    StartDate = DateTime.UtcNow,
                    DisplayGroupId = 1
                },
                new AuctionSession
                {
                    AuctionSessionId = 2,
                    StartDate = DateTime.UtcNow,
                    DisplayGroupId = 2
                }
            };

            _displayGroupRepository.Setup(a => a.GetPagedDisplayGroups())
                .Returns(displayGroupList);
            _auctionSessionRepository.Setup(a => a.GetAuctionSessionsOccuringOnDate(It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .Returns<DateTimeOffset, string>((date, tz) => Task.FromResult(auctionSessionList.Where(d => d.StartDate.HasValue && d.StartDate.Value.Day == date.Day)));

            //Action
            var testDisplayGroups = await _displayGroupsService.GetPagedDisplayGroups(pageNumber, pageSize, auctionSessionId, venueId, timeZone);

            //Result
            Assert.AreEqual(3, testDisplayGroups.List.Count());
        }

        [TestMethod]
        public async Task Get_PagedDisplayGroups_With_Empty_Results()
        {
            //Setup
            int pageNumber = 1;
            int pageSize = 3;
            int? auctionSessionId = 1;
            int? venueId = null;
            string timeZone = "New Zealand Standard Time";
            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    StartDate = DateTime.UtcNow,
                    DisplayGroupId = 1
                },
                new AuctionSession
                {
                    AuctionSessionId = 2,
                    StartDate = DateTime.UtcNow,
                    DisplayGroupId = 2
                }
            };

            var displayGroupList = new List<DisplayGroup>().AsQueryable();

            _displayGroupRepository.Setup(a => a.GetPagedDisplayGroups())
                .Returns(displayGroupList);
            _auctionSessionRepository.Setup(a => a.GetAuctionSessionsOccuringOnDate(It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .Returns<DateTimeOffset, string>((date, tz) => Task.FromResult(auctionSessionList.Where(d => d.StartDate.HasValue && d.StartDate.Value.Day == date.Day)));


            //Action
            var result = await _displayGroupsService.GetPagedDisplayGroups(pageNumber, pageSize, auctionSessionId, venueId, timeZone);

            //Result
            Assert.IsFalse(result.IsCancelled);
            Assert.IsFalse(result.List.Any());
        }

        [TestMethod]
        public async Task Get_PagedDisplayGroups_With_Venue_Id()
        {
            //Setup
            int pageNumber = 1;
            int pageSize = 3;
            int? auctionSessionId = 1;
            int? venueId = 1;
            string timeZone = "New Zealand Standard Time";

            var displayGroupList = new List<DisplayGroup>
            {
                new DisplayGroup
                {
                    DisplayGroupId = 1,
                    Name = "Test DisplayGroup 1",
                    VenueId = 1
                },
                new DisplayGroup
                {
                    DisplayGroupId = 2,
                    Name = "Test DisplayGroup 2",
                    VenueId = 2
                },
                new DisplayGroup
                {
                    DisplayGroupId = 3,
                    Name = "Test DisplayGroup 3",
                    VenueId = 2
                },
                new DisplayGroup
                {
                    DisplayGroupId = 4,
                    Name = "Test DisplayGroup 4",
                    VenueId = 2
                },
                new DisplayGroup
                {
                    DisplayGroupId = 5,
                    Name = "Test DisplayGroup 5",
                    VenueId = 1
                }
            }.AsQueryable();

            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    StartDate = DateTime.UtcNow,
                    DisplayGroupId = 1
                },
                new AuctionSession
                {
                    AuctionSessionId = 2,
                    StartDate = DateTime.UtcNow,
                    DisplayGroupId = 2
                }
            };

            _displayGroupRepository.Setup(a => a.GetPagedDisplayGroups())
                .Returns(displayGroupList);
            _auctionSessionRepository.Setup(a => a.GetAuctionSessionsOccuringOnDate(It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .Returns<DateTimeOffset, string>((date, tz) => Task.FromResult(auctionSessionList.Where(d => d.StartDate.HasValue && d.StartDate.Value.Day == date.Day)));

            //Action
            var testDisplayGroups = await _displayGroupsService.GetPagedDisplayGroups(pageNumber, pageSize, auctionSessionId, venueId, timeZone);

            //Result
            Assert.AreEqual(2, testDisplayGroups.List.Count());
        }

        [TestMethod]
        public async Task DisplayGroup_Delete_Valid_Group()
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
            await _displayGroupsService.DeleteDisplayGroup(0);

            //Result
            Assert.AreEqual(displayGroupList.Count, 0);
        }
    }
}
