using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services
{
    [TestClass]
    public class DisplaysServiceTests
    {
        private DisplayService _displaysService;
        private Mock<ILogger<Display>> _logger;
        private Mock<IDisplayRepository> _displayRepository;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<IBidRepository> _bidRepository;
        private Mock<IMediaRepository> _mediaRepository;
        private List<Display> _displayList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Display>>();
            _displayRepository = new Mock<IDisplayRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _bidRepository = new Mock<IBidRepository>();
            _mediaRepository = new Mock<IMediaRepository>();
            var serviceOptions = Options.Create(new ServiceSettings());
            serviceOptions.Value.DisplaySecret = "test";
            _displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 0,
                    DisplayName = "Display  1",
                    VenueId = 1
                },
                new Display
                {
                    DisplayId = 1,
                    DisplayName = "Display  2",
                    VenueId = 1
                }
            };

            var mockDataProtector = new Mock<IDataProtector>();
            mockDataProtector.Setup(sut => sut.Protect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes("protectTest"));
            mockDataProtector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes("1"));

            var mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
            mockDataProtectionProvider.Setup(s => s.CreateProtector(It.IsAny<string>())).Returns(mockDataProtector.Object);

            _displaysService = new DisplayService(_logger.Object, _displayRepository.Object, _venueRepository.Object, _auctionSessionRepository.Object, _bidRepository.Object, _mediaRepository.Object, Mapper.Instance, serviceOptions, mockDataProtectionProvider.Object);
        }

        [TestMethod]
        public async Task Get_Displays_By_Venue_Id_With_Valid_Venue_Id_Requested()
        {
            //Setup
            _displayRepository.Setup(v => v.GetDisplaysByVenueId(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_displayList.Where(d => d.VenueId == id)));

            //Action
            var testDisplays = await _displaysService.GetDisplaysByVenueId(1);

            //Result
            Assert.AreEqual(2, testDisplays.Count());
        }

        [TestMethod]
        public async Task Get_Displays_By_Venue_Id_With_Invalid_Venue_Id_Requested()
        {
            //Setup
            _displayRepository.Setup(v => v.GetDisplaysByVenueId(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_displayList.Where(d => d.VenueId == id)));

            //Action
            var testDisplays = await _displaysService.GetDisplaysByVenueId(0);

            //Result
            Assert.AreEqual(0, testDisplays.Count());
        }

        [TestMethod]
        public async Task Get_Available_Displays_By_Auction_Session_Id()
        {
            //Setup
            //construct all the relationships for auction sessions, displays, display configurations and display groups
            var auctionSession = new AuctionSession()
            {
                AuctionSessionId = 1,
                Name = "Auction Session Name",
                StartDate = DateTime.UtcNow
            };

            var display = new Display()
            {
                DisplayId = 1
            };

            var displayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 1,
                Display = display,
                DisplayId = 1
            };

            var displayGroupConfiguration = new DisplayGroupConfigurations
            {
                DisplayGroupId = 1,
                DisplayConfigurationId = 1,
                DisplayConfiguration = displayConfiguration
            };

            displayConfiguration.DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
            {
                displayGroupConfiguration
            };

            display.DisplayConfigurations = new List<DisplayConfiguration>
            {
                displayConfiguration
            };

            var auctionSessionList = new List<AuctionSession>()
            {
                auctionSession
            };

            auctionSession.DisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 1,
                AuctionSessions = auctionSessionList,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    displayGroupConfiguration
                }
            };

            displayGroupConfiguration.DisplayGroup = auctionSession.DisplayGroup;

            var displayList = new List<Display>()
            {
                display
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(auctionSessionList.Single(d => d.AuctionSessionId == id)));

            _displayRepository.Setup(v => v.GetDisplays())
                .Returns(Task.FromResult((IEnumerable<Display>)displayList));

            //Action
            var testDisplays = await _displaysService.GetAvailableDisplays(1);

            //Result
            Assert.AreEqual(1, testDisplays.Count());
        }

        [TestMethod]
        public void Unprotect_Valid_Display_ID()
        {
            //Action
            var displayId = _displaysService.UnprotectDisplayAccessToken("cHJvdGVjdFRlc3Q");

            //Result
            Assert.AreEqual("1", displayId);
        }

        [TestMethod]
        public void Unprotect_Invalid_Display_ID()
        {
            try
            {
                //Action
                _displaysService.UnprotectDisplayAccessToken("tokenHere");
                Assert.Fail();
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        [TestMethod]
        public void Protect_Valid_Display_ID()
        {
            //Action
            var accessToken = _displaysService.ProtectDisplayAccessToken(1);

            //Result
            Assert.AreEqual("cHJvdGVjdFRlc3Q", accessToken);
        }

        [TestMethod]
        public async Task Get_Displays_Allocated_Auction_Sessions_With_Valid_DisplayToken_And_Auction_Session_ID()
        {
            //Setup
            //construct all the relationships for auction sessions, displays, display configurations and display groups
            var auctionSession = new AuctionSession()
            {
                AuctionSessionId = 1,
                Name = "Auction Session Name",
                StartDate = DateTime.UtcNow
            };

            var display = new Display()
            {
                DisplayId = 1
            };

            var displayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 1,
                Display = display,
                DisplayId = 1
            };

            var displayGroupConfiguration = new DisplayGroupConfigurations
            {
                DisplayGroupId = 1,
                DisplayConfigurationId = 1,
                DisplayConfiguration = displayConfiguration
            };

            displayConfiguration.DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
            {
                displayGroupConfiguration
            };

            display.DisplayConfigurations = new List<DisplayConfiguration>
            {
                displayConfiguration
            };

            var auctionSessionList = new List<AuctionSession>()
            {
                auctionSession
            };

            auctionSession.DisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 1,
                AuctionSessions = auctionSessionList,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    displayGroupConfiguration
                }
            };

            displayGroupConfiguration.DisplayGroup = auctionSession.DisplayGroup;

            var displayList = new List<Display>()
            {
                display
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionDetailsById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(auctionSessionList.FirstOrDefault(d => d.AuctionSessionId == id)));

            _displayRepository.Setup(v => v.GetDisplays())
                .Returns(Task.FromResult((IEnumerable<Display>)displayList));

            //Action
            var testSessions = await _displaysService.GetDisplayAuctionSession("cHJvdGVjdFRlc3Q", 1);

            //Result
            Assert.IsNotNull(testSessions);
        }

        [TestMethod]
        public async Task Get_Displays_Allocated_Auction_Sessions_With_Valid_DisplayToken_And_Auction_Session_ID_But_Invalid_Not_Current_Session_DateTime()
        {
            //Setup
            //construct all the relationships for auction sessions, displays, display configurations and display groups
            var auctionSession = new AuctionSession()
            {
                AuctionSessionId = 1,
                Name = "Auction Session Name"
            };

            var display = new Display()
            {
                DisplayId = 1
            };

            var displayConfiguration = new DisplayConfiguration
            {
                DisplayConfigurationId = 1,
                Display = display,
                DisplayId = 1
            };

            var displayGroupConfiguration = new DisplayGroupConfigurations
            {
                DisplayGroupId = 1,
                DisplayConfigurationId = 1,
                DisplayConfiguration = displayConfiguration
            };

            displayConfiguration.DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
            {
                displayGroupConfiguration
            };

            display.DisplayConfigurations = new List<DisplayConfiguration>
            {
                displayConfiguration
            };

            var auctionSessionList = new List<AuctionSession>()
            {
                auctionSession
            };

            auctionSession.DisplayGroup = new DisplayGroup
            {
                DisplayGroupId = 1,
                AuctionSessions = auctionSessionList,
                DisplayGroupConfigurations = new List<DisplayGroupConfigurations>
                {
                    displayGroupConfiguration
                }
            };

            displayGroupConfiguration.DisplayGroup = auctionSession.DisplayGroup;

            var displayList = new List<Display>()
            {
                display
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionsOccuringOnDate(It.IsAny<DateTimeOffset?>(), It.IsAny<string>()))
                .Returns<DateTime, string>((date, tz) => Task.FromResult(auctionSessionList.Where(d => d.StartDate.HasValue && d.StartDate.Value.Day == date.Day)));

            _displayRepository.Setup(v => v.GetDisplays())
                .Returns(Task.FromResult((IEnumerable<Display>)displayList));

            //Action
            var testSessions = await _displaysService.GetDisplayAuctionSession("cHJvdGVjdFRlc3Q", 1);

            //Result
            Assert.IsNull(testSessions);
        }

        [TestMethod]
        public async Task Get_Latest_Bids_With_Valid_DisplayToken_And_Lot_ID()
        {
            //Setup
            //construct all the relationships for auction sessions, displays, display configurations and display groups
            var bids = new List<Bid>
            {
                new Bid
                {
                    Amount = 100000,
                    BidId = 1,
                    IsRejected = false,
                    LotId = 1
                },
                new Bid
                {
                    Amount = 125000,
                    BidId = 2,
                    IsRejected = true,
                    LotId = 1
                },
                new Bid
                {
                    Amount = 135000,
                    BidId = 3,
                    IsRejected = false,
                    LotId = 1
                },
                new Bid
                {
                    Amount = 170000,
                    BidId = 4,
                    IsRejected = true,
                    LotId = 1
                },
                new Bid
                {
                    Amount = 180000,
                    BidId = 5,
                    IsRejected = false,
                    LotId = 1
                },
                new Bid
                {
                    Amount = 200000,
                    BidId = 6,
                    IsRejected = false,
                    LotId = 1
                }
            };

            _bidRepository.Setup(b => b.GetLatestBidsByLotId(It.IsAny<int>()))
                .Returns<int>(lotId => Task.FromResult(bids.Where(b => b.LotId == lotId && !b.IsRejected).OrderByDescending(b => b.BidId).Take(3)));

            //Action
            var testBids = await _displaysService.GetDisplayBids("cHJvdGVjdFRlc3Q", 1);

            //Result
            Assert.IsNotNull(testBids);
            Assert.AreEqual(3, testBids.Count());
            Assert.AreEqual(200000, testBids.ElementAt(0).Amount);
            Assert.AreEqual(180000, testBids.ElementAt(1).Amount);
            Assert.AreEqual(135000, testBids.ElementAt(2).Amount);
        }

        [TestMethod]
        public async Task Get_Display_By_Id_Found_In_System_With_Valid_Fields()
        {
            //Setup
            _displayRepository.Setup(v => v.GetDisplayById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_displayList.Single(v => v.DisplayId == id)));

            //Action
            var displayResult = await _displaysService.GetDisplayById(1);

            //Result
            Assert.IsFalse(displayResult.IsCancelled);
            Assert.AreEqual("Display  2", displayResult.DisplayName);  
        }

        [TestMethod]
        public async Task Update_Display_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var displayDto = new DisplayDto { DisplayName = "Test" };

            _displayRepository.Setup(v => v.GetDisplayById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_displayList.Single(v => v.DisplayId == id)));

            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()))
                .Returns((int id, Display display) => Task.FromResult(display))
                .Callback<int, Display>((i, v) =>
                    _displayList.Single(li => li.DisplayId == v.DisplayId).DisplayName = v.DisplayName);

            //Action
            var displayResult = await _displaysService.UpdateDisplay(1, displayDto);

            //Result
            Assert.AreEqual("Test", displayResult.DisplayName);
            Assert.IsFalse(displayResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_Display_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var displayDto = new DisplayDto { DisplayName = "Test Display " };
            _displayRepository.Setup(v => v.GetDisplayById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_displayList.Single(v => v.DisplayId == id)));
            _displayRepository.Setup(a => a.UpdateDisplay(It.IsAny<int>(), It.IsAny<Display>()));

            //Action
            var displayResult = await _displaysService.UpdateDisplay(2, displayDto);

            //Result
            Assert.IsTrue(displayResult.IsCancelled);
        }

        [TestMethod]
        public async Task Add_Invalid_Display_To_System_No_Display_Details_Provided()
        {
            //Setup
            var displayToAddDto = new DisplayDto
            {

            };

            _displayRepository.Setup(a => a.AddDisplay(It.IsAny<Display>()));

            //Action
            var displayAdded = await _displaysService.AddDisplay(displayToAddDto);

            //Result
            Assert.IsTrue(displayAdded.IsCancelled);
            Assert.AreEqual(2, _displayList.Count);
        }

        [TestMethod]
        public async Task Add_Valid_Display_To_System_With_Display_Details_Provided()
        {
            //Setup
            var displayToAddDto = new DisplayDto
            {
                DisplayName = "Test Display "
            };

            _displayRepository.Setup(a => a.AddDisplay(It.IsAny<Display>()))
                .Returns(Task.FromResult(new Display()));

            //Action
            var displayAdded = await _displaysService.AddDisplay(displayToAddDto);

            //Result
            Assert.IsFalse(displayAdded.IsCancelled);
        }

        [TestMethod]
        public async Task Display_Delete_Valid_()
        {
            //Setup
            var testDisplay = new Display
            {
                DisplayId = 0,
                DisplayName = "Test Display"
            };
            var displayList = new List<Display>
            {
                new Display
                {
                    DisplayId = 0,
                    DisplayName = "Test Display to remove"
                }
            };

            _displayRepository.Setup(a => a.GetDisplayById(It.IsAny<int>()))
               .Returns(Task.FromResult(testDisplay));
            _displayRepository.Setup(a => a.DeleteDisplay(It.IsAny<int>()))
                .Returns(Task.FromResult(testDisplay))
                .Callback<int>(id => displayList.Remove(displayList.Single(listItem => listItem.DisplayId == id)));


            //Action
            await _displaysService.DeleteDisplay(0);

            //Result
            Assert.AreEqual(displayList.Count, 0);
        }


        [TestMethod]
        public async Task Get_Display_Media_With_Valid_Request()
        {
            //Setup
            var media = new List<Media>
            {
                new Media
                {
                    MediaId = 1,
                    Title = "Test Media"
                }
            };

            _mediaRepository.Setup(m => m.GetMedia()).Returns(Task.FromResult((IEnumerable<Media>) media));


            //Action
            var mediaResult = await _displaysService.GetDisplayMedia("cHJvdGVjdFRlc3Q");

            //Result
            Assert.AreEqual(mediaResult.Count(), 1);
        }
    }
}
