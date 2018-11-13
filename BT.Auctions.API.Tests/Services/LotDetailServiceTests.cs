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
    public class LotDetailServiceTests
    {
        private LotDetailService _lotDetailsService;
        private Mock<ILogger<LotDetail>> _logger;
        private Mock<ILotDetailRepository> _lotDetailRepository;
        private Mock<ILotRepository> _lotRepository;
        private List<LotDetail> _lotDetailList;
        private List<Lot> _lotList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<LotDetail>>();
            _lotDetailRepository = new Mock<ILotDetailRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _lotDetailList = new List<LotDetail>
            {
                new LotDetail
                {
                    LotDetailId = 1,
                    LotId = 1,
                    Key = "AgentName1",
                    Value = "Ball"
                },
                new LotDetail
                {
                    LotDetailId = 2,
                    LotId = 2,
                    Key = "AgentName1",
                    Value = "Chong"
                },
            };

            _lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 1,
                    Address = "test lot"
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_lotList.Single(lot => lot.LotId == id)));
            _lotDetailRepository.Setup(ld => ld.GetLotDetailsByLotIdAndKey(It.IsAny<int>(), It.IsAny<string>()))
                .Returns<int, string>((id, key) =>
                    Task.FromResult(_lotDetailList.FirstOrDefault(detail => detail.Key == key && detail.LotId == id)));
            _lotDetailRepository.Setup(v => v.GetLotDetailById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_lotDetailList.Single(v => v.LotDetailId == id)));
            _lotDetailsService = new LotDetailService(_logger.Object, _lotDetailRepository.Object, _lotRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public async Task Update_LotDetail_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var lotDetailDto = new LotDetailDto {Key = "Test LotDetail"};
            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()))
                .Returns((int id, LotDetail lotDetail) => Task.FromResult(lotDetail))
                .Callback<int, LotDetail>((i, v) => _lotDetailList.Single(li => li.LotDetailId == v.LotDetailId).Key = v.Key);

            //Action
            var lotDetailResult = await _lotDetailsService.UpdateLotDetail(1, lotDetailDto);

            //Result
            Assert.AreEqual(lotDetailResult.Key, "Test LotDetail");
            Assert.IsFalse(lotDetailResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_LotDetail_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var lotDetailDto = new LotDetailDto { Key = "Test LotDetail" };
            
            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()));

            //Action
            var lotDetailResult = await _lotDetailsService.UpdateLotDetail(3, lotDetailDto);

            //Result
            Assert.IsTrue(lotDetailResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_LotDetail_Not_Found_In_System_With_Invalid_Fields()
        {
            //Setup
            var lotDetailDto = new LotDetailDto { LotId = 1, Key = "AgentName1" };

            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()));

            //Action
            var lotDetailResult = await _lotDetailsService.UpdateLotDetail(2, lotDetailDto);

            //Result
            Assert.IsTrue(lotDetailResult.IsCancelled);
        }

        [TestMethod]
        public async Task Add_Invalid_LotDetail_To_System_No_LotDetail_Details_Provided()
        {
            //Setup
            var lotDetailToAddDto = new LotDetailDto();
            _lotDetailRepository.Setup(a => a.AddLotDetail(It.IsAny<LotDetail>()));

            //Action
            var lotDetailAdded = await _lotDetailsService.AddLotDetail(lotDetailToAddDto);

            //Result
            Assert.IsTrue(lotDetailAdded.IsCancelled);
            Assert.AreEqual(2, _lotDetailList.Count);
        }

        [TestMethod]
        public async Task Get_LotDetail_Valid_Lot()
        {
            //Setup  

            //Action
            var lotFound = await _lotDetailsService.GetLotDetailByLotId(1);

            //Result
            Assert.IsNotNull(lotFound);
        }

        [TestMethod]
        public async Task LotDetail_Delete_Valid_Detail()
        {
            //Setup
            _lotDetailRepository.Setup(a => a.DeleteLotDetail(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_lotDetailList.Remove(_lotDetailList.Single(listItem => listItem.LotDetailId == id))));


            //Action
            var deletedDetailResult = await _lotDetailsService.DeleteLotDetail(1);

            //Result
            Assert.IsFalse(deletedDetailResult.IsCancelled);
            Assert.AreEqual(_lotDetailList.Count, 1);
        }

        [TestMethod]
        public async Task LotDetail_Delete_Invalid_Detail_Not_In_System()
        {
            //Setup
            _lotDetailRepository.Setup(a => a.DeleteLotDetail(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_lotDetailList.Remove(_lotDetailList.Single(listItem => listItem.LotDetailId == id))));


            //Action
            var deletedDetailResult = await _lotDetailsService.DeleteLotDetail(100);

            //Result
            Assert.IsTrue(deletedDetailResult.IsCancelled);
            Assert.AreEqual(_lotDetailList.Count, 2);
        }
    }
}
