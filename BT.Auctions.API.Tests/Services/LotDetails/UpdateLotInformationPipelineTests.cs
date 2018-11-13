using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.LotDetails
{
    /// <summary>
    /// Update LotDetail pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateLotDetailPipelineTests
    {
        private UpdateLotDetailPipeline _pipeline;
        private Mock<ILotDetailRepository> _lotDetailRepository;
        private Mock<ILotRepository> _lotRepository;
        private Mock<ILogger<LotDetail>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<LotDetail>>();
            _lotDetailRepository = new Mock<ILotDetailRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _pipeline = new UpdateLotDetailPipeline(_logger.Object, _lotDetailRepository.Object, _lotRepository.Object);
        }

        [TestMethod]
        public async Task LotDetail_Update_Pipeline_Valid_LotDetail()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotId = 1,
                LotDetailId = 0,
                Key = "Updated LotDetail",
                Value = "lotDetail@lotDetail.com"
            };
            var lotDetailList = new List<LotDetail>
            {
                new LotDetail
                {
                    LotId = 1,
                    LotDetailId = 0,
                    Value = "lotDetail@lotDetail.com",
                    Key = "LotDetail To Be Updated"
                }
            };

            _lotDetailRepository.Setup(a => a.UpdateLotDetail(It.IsAny<int>(), It.IsAny<LotDetail>()))
                .Returns((int id, LotDetail lotDetail) => Task.FromResult(lotDetail))
                .Callback<int, LotDetail>((i, v) => lotDetailList.Single(li => li.LotDetailId == v.LotDetailId).Key = v.Key);
            _lotDetailRepository.Setup(a => a.GetLotDetailById(It.IsAny<int>()))
               .Returns(Task.FromResult(testLotDetail));


            //Action
            var updatedLotDetail = await _pipeline.Execute(testLotDetail);

            //Result
            Assert.IsFalse(updatedLotDetail.IsCancelled);
            Assert.AreEqual("Updated LotDetail", lotDetailList[0].Key);
        }
    }
}
