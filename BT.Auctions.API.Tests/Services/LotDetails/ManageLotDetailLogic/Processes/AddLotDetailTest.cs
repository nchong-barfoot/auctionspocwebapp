using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Add lotDetail pipeline process tests
    /// </summary>
    [TestClass]
    public class AddLotDetailTests
    {
        private AddLotDetail _addLotDetail;
        private Mock<ILotDetailRepository> _lotDetailRepository;

        [TestInitialize]
        public void Initialize()
        {
            _lotDetailRepository = new Mock<ILotDetailRepository>();
            _addLotDetail = new AddLotDetail(_lotDetailRepository.Object);
        }

        [TestMethod]
        public async Task LotDetail_Add_Valid_LotDetail_To_LotDetail_List()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                Key = "Test LotDetail"
            };
            var lotDetailList = new List<LotDetail>();
            _lotDetailRepository.Setup(a => a.AddLotDetail(It.IsAny<LotDetail>()))
                .Returns(Task.FromResult(testLotDetail))
                .Callback<LotDetail>(v => lotDetailList.Add(v));

            //Action
            await _addLotDetail.Execute(testLotDetail);

            //Result
            Assert.AreEqual(lotDetailList.Count, 1);
        }

        [TestMethod]
        public async Task LotDetail_Add_Invalid_LotDetail()
        {
            //Setup
            LotDetail testLotDetail = null;
            var lotDetailList = new List<LotDetail>();

            _lotDetailRepository.Setup(a => a.AddLotDetail(It.IsAny<LotDetail>()))
                .Returns(Task.FromResult(testLotDetail))
                .Callback<LotDetail>(v => lotDetailList.Add(v));

            //Action
            await _addLotDetail.Execute(testLotDetail);

            //Result
            Assert.AreEqual(lotDetailList.Count, 1);
        }
    }
}