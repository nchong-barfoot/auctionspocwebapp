using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Validate Venue Exists pipeline process testing
    /// </summary>
    [TestClass]
    public class ValidateAuctionSessionDisplayGroupsTest
    {
        private ValidateAuctionSessionDisplayGroups _validateAuctionSessionDisplayGroups;
        private Mock<IDisplayGroupRepository> _displayGroupRepository;

        [TestInitialize]
        public void Initialize()
        {
            _displayGroupRepository = new Mock<IDisplayGroupRepository>();
            _validateAuctionSessionDisplayGroups = new ValidateAuctionSessionDisplayGroups(_displayGroupRepository.Object);
        }

        [TestMethod]
        public async Task Auction_Session_Validate_Valid_DisplayGroup_Exists()
        {
            //Setup
            var testSession = new AuctionSession
            {
                AuctionSessionId = 0,
                VenueId = 0,
                Name = "Test Session",
                DisplayGroup = new DisplayGroup
                {
                    DisplayGroupId = 0
                }
            };
            
            var testSesssions = new List<AuctionSession>
            {
                testSession
            };

            var testGroup = new DisplayGroup
            {
                DisplayGroupId = 0
            };

            _displayGroupRepository.Setup(d => d.GetDisplayGroupById(It.IsAny<int>()))
                .Returns(Task.FromResult(testGroup));


            //Action
            await _validateAuctionSessionDisplayGroups.Execute(testSession);

            //Result
            Assert.IsFalse(testSession.IsCancelled);
        }

        [TestMethod]
        public async Task Auction_Session_Validate_Invalid_DisplayGroup_Exists_But_Another_Auction_Session_Has_Booked_The_Display_Group_For_That_Time()
        {
            //Setup
            var testSession = new AuctionSession
            {
                AuctionSessionId = 0,
                VenueId = 0,
                Name = "Test Session",
                StartDate = DateTime.UtcNow,
                FinishDate = DateTime.UtcNow.AddDays(1),
                DisplayGroup = new DisplayGroup
                {
                    DisplayGroupId = 0
                }
            };

            var testSesssions = new List<AuctionSession>
            {
                testSession,
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    VenueId = 0,
                    Name = "Test Session",
                    DisplayGroupId = 0,
                    StartDate = DateTime.UtcNow,
                    FinishDate = DateTime.UtcNow.AddDays(1),
                    IsInSession = true,
                    DisplayGroup = new DisplayGroup
                    {
                        DisplayGroupId = 0
                    }
                }
            };

            var testGroup = new DisplayGroup
            {
                DisplayGroupId = 0,
                AuctionSessions = testSesssions
            };

            _displayGroupRepository.Setup(d => d.GetDisplayGroupById(It.IsAny<int>()))
                .Returns(Task.FromResult(testGroup));

            //Action
            await _validateAuctionSessionDisplayGroups.Execute(testSession);

            //Result
            Assert.IsTrue(testSession.IsCancelled);
        }


        [TestMethod]
        public async Task Auction_Session_Validate_Invalid_DisplayGroup_Doesnt_Exist()
        {
            //Setup
            var testSession = new AuctionSession
            {
                AuctionSessionId = 0,
                VenueId = 0,
                Name = "Test Session",
                DisplayGroup = new DisplayGroup
                {
                    DisplayGroupId = 0
                }
            };

            var testSesssions = new List<AuctionSession>
            {
                testSession,
                new AuctionSession
                {
                    AuctionSessionId = 1,
                    VenueId = 0,
                    Name = "Test Session",
                    DisplayGroupId = 0,
                    DisplayGroup = new DisplayGroup
                    {
                        DisplayGroupId = 0
                    }
                }
            };

            var testGroup = new DisplayGroup
            {
                DisplayGroupId = 0
            };
            
            _displayGroupRepository.Setup(d => d.GetDisplayGroupById(It.IsAny<int>()))
                .Throws(new InvalidOperationException());


            //Action
            await _validateAuctionSessionDisplayGroups.Execute(testSession);

            //Result
            Assert.IsTrue(testSession.IsCancelled);
        }
    }
}