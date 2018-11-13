using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Data.Interfaces;
using Moq;

namespace BT.Auctions.API.Tests.Services.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Used to test the lot has address pipeline process
    /// </summary>
    [TestClass]
    public class ValidateLotDetailKeyAndLotIsUniqueTest
    {
        private ValidateLotDetailKeyAndLotIsUnique _validateLotDetailKeyAndLotIsUnique;
        private Mock<ILotDetailRepository> _lotDetailRepository;
        private List<LotDetail> _lotDetailList;

        [TestInitialize]
        public void Initialize()
        {
            _lotDetailRepository = new Mock<ILotDetailRepository>();
            _lotDetailList = new List<LotDetail>
            {
                new LotDetail
                {
                    LotDetailId = 1,
                    LotId = 1,
                    Key = "Name",
                    Value = "Ball"
                },
                new LotDetail
                {
                    LotDetailId = 2,
                    LotId = 2,
                    Key = "Name",
                    Value = "Chong"
                }
            };

            _lotDetailRepository.Setup(ld => ld.GetLotDetailsByLotIdAndKey(It.IsAny<int>(), It.IsAny<string>()))
                .Returns<int, string>((id, key) =>
                    Task.FromResult(_lotDetailList.FirstOrDefault(detail => detail.Key == key && detail.LotId == id)));
            _validateLotDetailKeyAndLotIsUnique = new ValidateLotDetailKeyAndLotIsUnique(_lotDetailRepository.Object);
        }

        [TestMethod]
        public async Task LotDetail_Validate_Valid_Unique_LotDetail()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotDetailId = 0
            };

            //Action
            await _validateLotDetailKeyAndLotIsUnique.Execute(testLotDetail);

            //Result
            //Expected no exception but returned test lot
        }

        [TestMethod]
        public async Task LotDetail_Validate_Valid_LotDetail_Not_Found_Null_LotDetail()
        {
            //Setup
            LotDetail testLotDetail = null;

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _validateLotDetailKeyAndLotIsUnique.Execute(testLotDetail));

            //Result
            //Expected a null reference exception
        }

        [TestMethod]
        public async Task LotDetail_Validate_Invalid_LotDetail_Duplicate_Key()
        {
            //Setup
            var testLotDetail = new LotDetail
            {
                LotId = 1,
                LotDetailId = 500,
                Key = "Name"
            };

            //Action
            await  _validateLotDetailKeyAndLotIsUnique.Execute(testLotDetail);

            //Result
            Assert.IsTrue(testLotDetail.IsCancelled);
        }
    }
}