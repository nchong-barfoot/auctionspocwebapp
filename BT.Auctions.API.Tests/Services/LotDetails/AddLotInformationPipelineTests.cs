using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.LotDetails
{
    /// <summary>
    /// Add LotDetail pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddLotDetailPipelineTests
    {
        private AddLotDetailPipeline _pipeline;
        private Mock<ILotDetailRepository> _lotDetailRepository;
        private Mock<ILotRepository> _lotRepository;
        private Mock<ILogger<LotDetail>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<LotDetail>>();
            _lotDetailRepository = new Mock<ILotDetailRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _pipeline = new AddLotDetailPipeline(_logger.Object, _lotDetailRepository.Object, _lotRepository.Object);
        }

        [TestMethod]
        public async Task LotDetail_Add_Pipeline_Valid_LotDetail()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotId = 1,
                LotDetailId = 0,
                Key = "Test LotDetail",
                Value = "lotDetail@lotDetail.com"
            };
            var lotDetailList = new List<LotDetail>();

            _lotDetailRepository.Setup(a => a.AddLotDetail(It.IsAny<LotDetail>()))
                .Returns(Task.FromResult(testLotDetail))
                .Callback<LotDetail>(v => lotDetailList.Add(v));

            //Action
            var addedLotDetail = await _pipeline.Execute(testLotDetail);

            //Result
            Assert.IsFalse(addedLotDetail.IsCancelled);
            Assert.AreEqual(1, lotDetailList.Count);
        }

        [TestMethod]
        public async Task LotDetail_Add_Pipeline_Invalid_LotDetail_No_Lot()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotDetailId = 0,
                Key = "TestLotDetail",
                Value = "LotDetail 007"
            };

            var lotList = new List<Lot>
            {
                new Lot
                {
                    LotId = 1,
                    Address = "Test Lot"
                }
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(lotList.Single(l => l.LotId == id)));
            _lotDetailRepository.Setup(a => a.AddLotDetail(It.IsAny<LotDetail>()))
                .Returns(Task.FromResult(testLotDetail));

            //Action
            var addedLotDetail = await _pipeline.Execute(testLotDetail);

            //Result
            Assert.IsTrue(addedLotDetail.IsCancelled);
        }

        [TestMethod]
        public async Task LotDetail_Add_Pipeline_Invalid_LotDetail_Null_Values()
        {
            //Setup
            var lotDetailList = new List<LotDetail>();

            _lotDetailRepository.Setup(a => a.AddLotDetail(It.IsAny<LotDetail>()))
                .Returns(Task.FromResult((LotDetail) null));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(null));

            //Result
            Assert.IsTrue(lotDetailList.Count == 0);
        }
    }
}