using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Update LotDetail pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateLotDetailTest 
    {
        private UpdateLotDetail _updateLotDetail;
        private Mock<ILotDetailRepository> _lotDetailRepository;

        [TestInitialize]
        public void Initialize()
        {
            _lotDetailRepository = new Mock<ILotDetailRepository>();
            _updateLotDetail = new UpdateLotDetail(_lotDetailRepository.Object);
        }


        [TestMethod]
        public async Task LotDetail_Update_Valid_LotDetail()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotDetailId = 0,
                Key = "Updated LotDetail"
            };

            var lotDetailList = new List<LotDetail>
            {
                new LotDetail
                {
                    LotDetailId = 0,
                    Key = "Test LotDetail"
                }
            };

            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()))
                .Returns((int id, LotDetail lotDetail) => Task.FromResult(lotDetail))
                .Callback<int, LotDetail>((i, v) => lotDetailList.Single(li => li.LotDetailId == v.LotDetailId).Key = v.Key);

            //Action
            await _updateLotDetail.Execute(testLotDetail);

            //Result
            Assert.AreEqual(lotDetailList.ElementAt(0).Key, "Updated LotDetail");
        }

        [TestMethod]
        public async Task LotDetail_Update_Invalid_LotDetail()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotDetailId = 1,
                Key = "Updated LotDetail"
            };

            var lotDetailList = new List<LotDetail>
            {
                new LotDetail
                {
                    LotDetailId = 0,
                    Key = "Test LotDetail"
                }
            };

            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()))
                .Returns((int id, LotDetail lotDetail) => Task.FromResult(lotDetail))
                .Callback<int, LotDetail>((i, v) => lotDetailList.Single(li => li.LotDetailId == v.LotDetailId).Key = v.Key);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateLotDetail.Execute(testLotDetail));
        }

        [TestMethod]
        public async Task LotDetail_Update_Invalid_LotDetail_No_LotDetails()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotDetailId = 0,
                Key = "Updated LotDetail"
            };

            var lotDetailList = new List<LotDetail>();

            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()))
                .Returns((int id, LotDetail lotDetail) => Task.FromResult(lotDetail))
                .Callback<int, LotDetail>((i, v) => lotDetailList.Single(li => li.LotDetailId == v.LotDetailId).Key = v.Key);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateLotDetail.Execute(testLotDetail));
        }

        [TestMethod]
        public async Task LotDetail_Update_No_Changes()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotDetailId = 0,
                Key = "Test LotDetail"
            };

            var lotDetailList = new List<LotDetail>
            {
                new LotDetail
                {
                    LotDetailId = 0,
                    Key = "Test LotDetail"
                }
            };

            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()))
                .Returns((int id, LotDetail lotDetail) => Task.FromResult(lotDetail))
                .Callback<int, LotDetail>((i, v) => lotDetailList.Single(li => li.LotDetailId == v.LotDetailId).Key = v.Key);

            //Action
            await _updateLotDetail.Execute(testLotDetail);

            //Result
            Assert.AreEqual(lotDetailList.ElementAt(0).Key, "Test LotDetail");
        }
    }
}