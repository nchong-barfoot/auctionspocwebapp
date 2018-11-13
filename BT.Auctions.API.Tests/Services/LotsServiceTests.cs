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
    public class LotsServiceTests
    {
        private LotService _lotsService;
        private Mock<ILogger<Lot>> _logger;
        private Mock<ILotRepository> _lotRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private Mock<ILotDetailRepository> _lotDetailRepository;
        private List<Lot> _lotList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Lot>>();
            _lotRepository = new Mock<ILotRepository>();
            _lotDetailRepository = new Mock<ILotDetailRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 0,
                    AuctionSessionId = 0,
                    AuctionStatus = AuctionStatus.NoBids,
                    Address = "Test Address",
                    FeatureDescription = "Features Description",
                    ListingId = 0,
                    PlusGST = false,
                    ReserveMet = false,
                    Order = 0,
                    PostponedDateTime = null,
                    SoldDate = null,
                    SoldPrice = null
                },
                new Lot
                {
                    LotId = 1,
                    AuctionSessionId = 0,
                    AuctionStatus = AuctionStatus.Sold,
                    Address = "Test Address Sold",
                    FeatureDescription = "Features Description",
                    ListingId = 0,
                    PlusGST = true,
                    ReserveMet = true,
                    Order = 0,
                    PostponedDateTime = null,
                    SoldDate = new DateTime(2018, 7, 6),
                    SoldPrice = 1200000
                }
            };

            _lotsService = new LotService(_logger.Object, _lotRepository.Object, _auctionSessionRepository.Object, _lotDetailRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public async Task Get_Lot_By_Auction_Session_Id_With_Valid_Lot_Requested()
        {
            //Setup
            _lotRepository.Setup(v => v.GetLotsByAuctionSessionId(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_lotList.Where(v => v.AuctionSessionId == id)));

            //Action
            var testLots = await _lotsService.GetLotsByAuctionSessionId(0);

            //Result
            Assert.AreEqual(2, testLots.Count());
        }

        [TestMethod]
        public async Task Get_All_Lots_In_System()
        {
            //Setup
            var lotTask = Task.FromResult(_lotList as IEnumerable<Lot>);
            _lotRepository.Setup(v => v.GetLots()).Returns(lotTask);

            //Action
            var lotListResult = await _lotsService.GetLots();

            //Result
            Assert.AreEqual(lotListResult.Count(), 2);
        }

        [TestMethod]
        public async Task Get_All_Lots_Empty_Lot_List_In_System()
        {
            //Setup
            var lotList = new List<Lot>();
            var lotTask = Task.FromResult((IEnumerable<Lot>)lotList);
            _lotRepository.Setup(v => v.GetLots()).Returns(lotTask);

            //Action
            var lotListResult = await _lotsService.GetLots();

            //Result
            Assert.AreEqual(lotListResult.Count(), 0);
        }

        [TestMethod]
        public async Task Update_Lot_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var lotDto = new LotDto { LotId = 1, Address = "Test Lot", SoldPrice = 500000, SoldDate = DateTime.UtcNow };
            _lotRepository.Setup(v => v.GetLotDetailsWithBidsById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_lotList.Single(v => v.LotId == id)));
            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()))
                .Returns((int id, Lot lot) => Task.FromResult(lot))
                .Callback<int, Lot>((i, v) => _lotList.Single(li => li.LotId == v.LotId).Address = v.Address);

            //Action
            var lotResult = await _lotsService.UpdateLot(1, lotDto);

            //Result
            Assert.AreEqual(lotResult.Address, "Test Lot");
            Assert.AreEqual(lotResult.AuctionStatus, AuctionStatus.Sold);
            Assert.IsFalse(lotResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_Lot_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var lotDto = new LotDto { Address = "Test Lot" };
            _lotRepository.Setup(v => v.GetLotDetailsWithBidsById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_lotList.Single(v => v.LotId == id)));
            _lotRepository.Setup(a => a.UpdateLot(It.IsAny<int>(), It.IsAny<Lot>()));

            //Action
            var lotResult = await _lotsService.UpdateLot(2, lotDto);

            //Result
            Assert.IsTrue(lotResult.IsCancelled);
        }

        [TestMethod]
        public async Task Add_Invalid_Lot_To_System_No_Lot_Details_Provided()
        {
            //Setup
            var lotToAddDto = new LotDto
            {

            };

            _lotRepository.Setup(a => a.AddLot(It.IsAny<Lot>()));

            //Action
            var lotAdded = await _lotsService.AddLot(lotToAddDto);

            //Result
            Assert.IsTrue(lotAdded.IsCancelled);
            Assert.AreEqual(2, _lotList.Count);
        }

        [TestMethod]
        public async Task Add_Valid_Lot_To_System_Lot_Details_Provided()
        {
            //Setup
            var lotToAddDto = new LotDto
            {
                Address = "Test Address",
                AuctionSessionId = 3,
                AuctionStatus = AuctionStatus.None,
                Order = 1,
                ListingId = 12,
                PlusGST = false,
                FeatureDescription = "Test Lot"
            };
            var testAuctionSession = new AuctionSession
            {
                AuctionSessionId = 3
            };

            _auctionSessionRepository.Setup(a => a.GetAuctionSessionById(It.IsAny<int>())).Returns(Task.FromResult(testAuctionSession));
            _lotRepository.Setup(a => a.AddLot(It.IsAny<Lot>())).Returns(Task.FromResult(new Lot()));

            //Action
            var lotAdded = await _lotsService.AddLot(lotToAddDto);

            //Result
            Assert.IsFalse(lotAdded.IsCancelled);
        }
    }
}
