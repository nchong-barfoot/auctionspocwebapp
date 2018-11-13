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
    public class BidsServiceTests
    {
        private BidService _bidsService;
        private Mock<ILogger<Bid>> _logger;
        private Mock<IBidRepository> _bidRepository;
        private Mock<ILotRepository> _lotRepository;
        private List<Bid> _bidList;
        private List<Lot> _lotList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Bid>>();
            _bidRepository = new Mock<IBidRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _bidList = new List<Bid>
            {
                new Bid
                {
                    BidId = 1,
                    LotId = 1,
                    Amount = 100,
                    IsRejected = false,
                    ReserveMet = false,
                },
                new Bid
                {
                    BidId = 2,
                    LotId = 1,
                    Amount = 200,
                    IsRejected = false,
                    ReserveMet = true
                },
                new Bid
                {
                    BidId = 3,
                    LotId = 1,
                    Amount = 300,
                    IsRejected = true,
                    ReserveMet = true
                }
            };
            _lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 1,
                    Address = "Otahuhu"
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns<int>(lotId => Task.FromResult(_lotList.Single(i => i.LotId == lotId)));

            _bidsService = new BidService(_logger.Object, _bidRepository.Object, _lotRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public async Task Get_Bid_By_Lot_Id_With_Valid_Bid_Requested()
        {
            //Setup
            _bidRepository.Setup(v => v.GetBidsByLotId(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_bidList.Where(v => v.LotId == id)));

            //Action
            var testBids = await _bidsService.GetBidsByLotId(1);

            //Result
            Assert.AreEqual(3, testBids.Count());
        }


        [TestMethod]
        public async Task Get_Latest_Bids_By_Lot_Id()
        {
            //Setup
            _bidRepository.Setup(v => v.GetLatestBidsByLotId(It.IsAny<int>()))
                .Returns<int>(lotId =>
                    Task.FromResult(_bidList.Where(b => b.LotId == lotId && !b.IsRejected).OrderByDescending(b => b.BidId).Take(3)));

            //Action
            var testBids = await _bidsService.GetLatestBidsByLotId(1);

            //Result
            Assert.AreEqual(2, testBids.Count());
        }

        [TestMethod]
        public async Task Update_Bid_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var bidDto = new BidDto {IsRejected = true};
            _bidRepository.Setup(a => a.UpdateBid(It.IsAny<int>(), It.IsAny<Bid>()))
                .Returns((int id, Bid bid) => Task.FromResult(bid))
                .Callback<int, Bid>((i, v) => _bidList.Single(li => li.BidId == v.BidId).IsRejected = v.IsRejected);
            _bidRepository.Setup(a => a.GetBid(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_bidList.Single(b => b.BidId == id)));

            //Action
            var bidResult = await _bidsService.UpdateBid(1, bidDto);

            //Result
            Assert.IsTrue(bidResult.IsRejected);
            Assert.IsFalse(bidResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_Bid_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var bidDto = new BidDto { Amount = 1 };
            _bidRepository.Setup(a => a.UpdateBid(It.IsAny<int>(), It.IsAny<Bid>()));
            _bidRepository.Setup(a => a.GetBid(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_bidList.Single(b => b.BidId == id)));

            //Action
            var bidResult = await _bidsService.UpdateBid(4, bidDto);

            //Result
            Assert.IsTrue(bidResult.IsCancelled);
        }
        
        [TestMethod]
        public async Task Add_Invalid_Bid_To_System_No_Bid_Details_Provided()
        {
            //Setup
            var bidToAddDto = new BidDto
            {

            };

            _bidRepository.Setup(a => a.AddBid(It.IsAny<Bid>()));

            //Action
            var bidAdded = await _bidsService.AddBid(bidToAddDto);

            //Result
            Assert.IsTrue(bidAdded.IsCancelled);
            Assert.AreEqual(3, _bidList.Count);
        }

        [TestMethod]
        public async Task Add_Valid_Bid_To_System_Bid_Details_Provided()
        {
            //Setup
            var bidToAddDto = new BidDto
            {
                Amount = 2,
                LotId = 3,
            };
            var testLot = new Lot
            {
                LotId = 3
            };

            var bid = new Bid
            {
                BidId = 1,
                Amount = 2,
                LotId = 3
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>())).Returns(Task.FromResult(testLot));
            _bidRepository.Setup(a => a.AddBid(It.IsAny<Bid>())).Returns(Task.FromResult(bid));

            //Action
            var bidAdded = await _bidsService.AddBid(bidToAddDto);

            //Result
            Assert.IsFalse(bidAdded.IsCancelled);
        }
    }
}
