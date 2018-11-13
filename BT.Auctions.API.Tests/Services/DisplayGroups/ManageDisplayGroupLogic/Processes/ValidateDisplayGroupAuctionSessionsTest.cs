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
    /// Validate DisplayGroup Auction Session Exists Tests
    /// </summary>
    [TestClass]
    public class ValidateDisplayGroupAuctionSessionsTest
    {
        private ValidateDisplayGroupAuctionSessions _validateDisplayGroupAuctionSessionsExist;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;

        [TestInitialize]
        public void Initialize()
        {
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _validateDisplayGroupAuctionSessionsExist = new ValidateDisplayGroupAuctionSessions(_auctionSessionRepository.Object);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Valid_DisplayGroup_With_AuctionSession_Found_Not_Allocated_To_Another_Group()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                AuctionSessions = new List<AuctionSession>
                {
                    new AuctionSession
                    {
                        AuctionSessionId = 0,
                        DisplayGroupId = null
                    }
                }
            };

            var testSession = new List<AuctionSession>
            {
                new AuctionSession
                {
                    DisplayGroupId = null
                }
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.AuctionSessionId == auctionSessionId)));

            //Action
            await _validateDisplayGroupAuctionSessionsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsFalse(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_With_AuctionSession_Found_Allocated_To_Another_Group()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                AuctionSessions = new List<AuctionSession>
                {
                    new AuctionSession
                    {
                        AuctionSessionId = 0,
                        DisplayGroupId = null
                    }
                }
            };

            var testSession = new List<AuctionSession>
            {
                new AuctionSession
                {
                    DisplayGroupId = 1,
                    AuctionSessionId = 0
                }
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.AuctionSessionId == auctionSessionId)));

            //Action
            await _validateDisplayGroupAuctionSessionsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Valid_DisplayGroup_With_Null_Auction_Session()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0
            };

            var testSessions = new List<AuctionSession>
            {
                new AuctionSession
                {
                    DisplayGroupId = 0
                }
            };

            //Action
            await _validateDisplayGroupAuctionSessionsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsFalse(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_With_AuctionSession_Not_Found()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                AuctionSessions = new List<AuctionSession>
                {
                    new AuctionSession
                    {
                        AuctionSessionId = 0,
                        DisplayGroupId = null
                    }
                }
            };

            var testSession = new List<AuctionSession>
            {
                new AuctionSession
                {
                    DisplayGroupId = 0,
                    AuctionSessionId = 1
                }
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSession.Single(i => i.AuctionSessionId == auctionSessionId)));

            //Action
            await _validateDisplayGroupAuctionSessionsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_With_AuctionSession_Found_With_Same_Date_Range_In_Provided_Collection()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                AuctionSessions = new List<AuctionSession>
                {
                    new AuctionSession
                    {
                        AuctionSessionId = 1,
                        DisplayGroupId = null,
                        StartDate = new DateTime(2018,07,25),
                        FinishDate = new DateTime(2018,07,26)
                    },
                    new AuctionSession
                    {
                        AuctionSessionId = 2,
                        DisplayGroupId = null,
                        StartDate = new DateTime(2018,07,25),
                        FinishDate = new DateTime(2018,07,26)
                    }
                }
            };

            var testSessions = new List<AuctionSession>
            {
                new AuctionSession
                {
                    DisplayGroupId = null,
                    AuctionSessionId = 1,
                    StartDate = new DateTime(2018,07,25),
                    FinishDate = new DateTime(2018,07,26)
                },
                new AuctionSession
                {
                    DisplayGroupId = null,
                    AuctionSessionId = 2,
                    StartDate = new DateTime(2018,07,25),
                    FinishDate = new DateTime(2018,07,26)
                }
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSessions.Single(i => i.AuctionSessionId == auctionSessionId)));

            _auctionSessionRepository
                .Setup(a => a.GetAuctionSessionsByDateTime(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));                

            //Action
            await _validateDisplayGroupAuctionSessionsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }

        [TestMethod]
        public async Task DisplayGroup_Validate_Invalid_DisplayGroup_With_AuctionSession_Found_Within_Date_Range_In_Provided_Collection()
        {
            //Setup
            var testDisplayGroup = new DisplayGroup()
            {
                DisplayGroupId = 0,
                AuctionSessions = new List<AuctionSession>
                {
                    new AuctionSession
                    {
                        AuctionSessionId = 1,
                        DisplayGroupId = null,
                        StartDate = new DateTime(2018,07,25),
                        FinishDate = new DateTime(2018,07,27)
                    },
                    new AuctionSession
                    {
                        AuctionSessionId = 2,
                        DisplayGroupId = null,
                        StartDate = new DateTime(2018,07,26),
                        FinishDate = new DateTime(2018,07,28)
                    }
                }
            };

            var testSessions = new List<AuctionSession>();

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>()))
               .Returns<int>(auctionSessionId => Task.FromResult(testSessions.Single(i => i.AuctionSessionId == auctionSessionId)));

            //Action
            await _validateDisplayGroupAuctionSessionsExist.Execute(testDisplayGroup);

            //Result
            Assert.IsTrue(testDisplayGroup.IsCancelled);
        }
    }
}