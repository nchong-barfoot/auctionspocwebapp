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
    public class AuctionSessionsServiceTests
    {
        private AuctionSessionService _auctionSessionsService;
        private Mock<ILogger<AuctionSession>> _logger;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<AuctionSession>>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _venueRepository = new Mock<IVenueRepository>();
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _venueRepository.Setup(v => v.GetVenueById(It.IsAny<int>()));
            _auctionSessionsService = new AuctionSessionService(_logger.Object, _auctionSessionRepository.Object, _venueRepository.Object, _displayGroupRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public async Task Get_AuctionSession_By_Id_With_Valid_AuctionSession_Requested()
        {
            //Setup
            var auctionSessionTest = new AuctionSession
                {
                    AuctionSessionId = 1,
                    Name = "Test AuctionSession 2"
                };
            _auctionSessionRepository.Setup(v => v.GetAuctionSessionDetailsById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(auctionSessionTest));

            //Action
            var testAuctionSession = await _auctionSessionsService.GetAuctionSessionById(1);

            //Result
            Assert.AreEqual("Test AuctionSession 2", testAuctionSession.Name);
        }

        [TestMethod]
        public async Task Get_AuctionSession_By_Id_Invalid_Not_Found_AuctionSession_Requested()
        {
            //Setup
            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    Name = "Test AuctionSession 2"
                }
            };
            _auctionSessionRepository.Setup(v => v.GetAuctionSessionDetailsById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(auctionSessionList.Single(v => v.AuctionSessionId == id)));

            //Action
            var auctionSessionRetrieved = await _auctionSessionsService.GetAuctionSessionById(2);

            //Result
            Assert.IsTrue(auctionSessionRetrieved.IsCancelled);
        }
        
        [TestMethod]
        public async Task Get_All_AuctionSessions_In_System()
        {
            //Setup
            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 0,
                    Name = "Test AuctionSession"
                },
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    Name = "Test AuctionSession 2"
                }
            };
            _auctionSessionRepository.Setup(v => v.GetAuctionSessionDetailsById(It.IsAny<int>()));
            var auctionSessionTask = Task.FromResult((IEnumerable<AuctionSession>) auctionSessionList);

            //Action
            _auctionSessionRepository.Setup(v => v.GetAuctionSessions()).Returns(auctionSessionTask);
            var auctionSessionListResult = await _auctionSessionsService.GetAuctionSessions();

            //Result
            Assert.AreEqual(auctionSessionListResult.Count(), 2);
        }

        [TestMethod]
        public async Task Get_All_AuctionSessions_Empty_AuctionSession_List_In_System()
        {
            //Setup
            var auctionSessionList = new List<AuctionSession>();
            var auctionSessionTask = Task.FromResult((IEnumerable<AuctionSession>)auctionSessionList);
            _auctionSessionRepository.Setup(v => v.GetAuctionSessions()).Returns(auctionSessionTask);

            //Action
            var auctionSessionListResult = await _auctionSessionsService.GetAuctionSessions();

            //Result
            Assert.AreEqual(auctionSessionListResult.Count(), 0);
        }

        [TestMethod]
        public async Task Get_PagedAuctionSessions_Empty_AuctionSession_List_In_System()
        {
            //Setup
            var currentTimeZone = "New Zealand Standard Time";
            var auctionSessionList = new List<AuctionSession>().AsQueryable();
            _auctionSessionRepository.Setup(v => v.GetPagedAuctionSessions(It.IsAny<DateTime?>(), It.IsAny<string>())).Returns(auctionSessionList);

            //Action
            var auctionSessionListResult = await _auctionSessionsService.GetPagedAuctionSessions(1, 1, currentTimeZone, null);

            //Result
            Assert.AreEqual(0, auctionSessionListResult.TotalItems);
        }

        [TestMethod]
        public async Task Get_PagedAuctionSessions_In_System()
        {
            //Setup
            var currentTimeZone = "New Zealand Standard Time";
            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 0,
                    Name = "Test AuctionSession",
                    StartDate = DateTime.UtcNow
                },
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    Name = "Test AuctionSession 2",
                    StartDate = DateTime.UtcNow
                }
            }.AsQueryable();
            _auctionSessionRepository.Setup(v => v.GetPagedAuctionSessions(It.IsAny<DateTimeOffset?>(), It.IsAny<string>())).Returns(auctionSessionList);

            //Action
            var auctionSessionListResult = await _auctionSessionsService.GetPagedAuctionSessions(1, 1, currentTimeZone, null);

            //Result
            Assert.AreEqual(1, auctionSessionListResult.TotalItems);
        }

        [TestMethod]
        public async Task Get_PagedAuctionSessions_In_System_With_Invalid_Timezone()
        {
            //Setup
            var currentTimeZone = "Invalid Standard Time";
            var auctionSessionList = new List<AuctionSession>().AsQueryable();
            _auctionSessionRepository.Setup(v => v.GetPagedAuctionSessions(It.IsAny<DateTimeOffset?>(), It.IsAny<string>())).Returns(auctionSessionList);

            //Action
            var auctionSessionListResult = await _auctionSessionsService.GetPagedAuctionSessions(1, 1, currentTimeZone, null);

            //Result
            Assert.IsTrue(auctionSessionListResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_AuctionSession_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var auctionSessionDto = new AuctionSessionDto { Name = "Test AuctionSession" };

            //Action
            var auctionSessionResult = await _auctionSessionsService.UpdateAuctionSession(1, auctionSessionDto);

            //Result
            Assert.AreEqual(auctionSessionResult.Name, "Test AuctionSession");
            Assert.IsFalse(auctionSessionResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_AuctionSession_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    Name = "Test AuctionSession 2"
                }
            };
            _auctionSessionRepository.Setup(v => v.GetAuctionSessionDetailsById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(auctionSessionList.Single(v => v.AuctionSessionId == id)));
            var auctionSessionDto = new AuctionSessionDto { Name = "Test AuctionSession" };

            //Action
            var auctionSessionResult = await _auctionSessionsService.UpdateAuctionSession(2, auctionSessionDto);

            //Result
            Assert.IsTrue(auctionSessionResult.IsCancelled);
        }

        [TestMethod]
        public async Task Add_Valid_AuctionSession_To_System()
        {
            //Setup
            var auctionSessionToAddDto = new AuctionSessionDto
            {
                Name = "Test AuctionSession 2"
            };
            var auctionSessionList = new List<AuctionSession>();

            _auctionSessionRepository.Setup(a => a.AddAuctionSession(It.IsAny<AuctionSession>()))
                .Returns<AuctionSession>(Task.FromResult)
                .Callback<AuctionSession>(s => auctionSessionList.Add(s));

            //Action
            var auctionSessionAdded = await _auctionSessionsService.AddAuctionSession(auctionSessionToAddDto);

            //Result
            Assert.IsFalse(auctionSessionAdded.IsCancelled);
            Assert.AreEqual(auctionSessionList.Count, 1);
        }

        [TestMethod]
        public async Task Get_Auction_Session_By_Date()
        {
            //Setup
            var auctionSessionList = new List<AuctionSession>
            {
                new AuctionSession
                {
                    AuctionSessionId = 0,
                    Name = "Test AuctionSession"
                },
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    Name = "Test AuctionSession 2"
                }
            };
            _auctionSessionRepository
                .Setup(a => a.GetAuctionSessionsByDateTime(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns<DateTime?, DateTime?>((startDate, finishDate) =>
                    Task.FromResult(auctionSessionList.Where(s =>
                        (startDate.HasValue && s.StartDate.HasValue && s.StartDate >= startDate ||
                         !startDate.HasValue)
                        && (finishDate.HasValue && s.FinishDate.HasValue && s.FinishDate <= finishDate ||
                            !finishDate.HasValue))));


            //Action
            var auctionSessionsRetrieved = await _auctionSessionsService.GetAuctionSessionsByDate(null, null);

            //Result
            Assert.AreEqual(2, auctionSessionsRetrieved.Count());
        }
    }
}
