using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Images.ManageImageLogic.Processes
{
    /// <summary>
    /// Add image pipeline process tests
    /// </summary>
    [TestClass]
    public class AddImageTests
    {
        private AddImage _addImage;
        private Mock<IImageRepository> _imageRepository;

        [TestInitialize]
        public void Initialize()
        {
            _imageRepository = new Mock<IImageRepository>();
            _addImage = new AddImage(_imageRepository.Object);
        }

        [TestMethod]
        public async Task Image_Add_Valid_Image_To_Image_List()
        {
            //Setup
            var testImage = new Image
            {
                Url = "Test Image"
            };
            var imageList = new List<Image>();
            _imageRepository.Setup(a => a.AddImage(It.IsAny<Image>()))
                .Returns(Task.FromResult(testImage))
                .Callback<Image>(v => imageList.Add(v));

            //Action
            await _addImage.Execute(testImage);

            //Result
            Assert.AreEqual(imageList.Count, 1);
        }

        [TestMethod]
        public async Task Image_Add_Invalid_Image()
        {
            //Setup
            Image testImage = null;
            var imageList = new List<Image>();

            _imageRepository.Setup(a => a.AddImage(It.IsAny<Image>()))
                .Returns(Task.FromResult(testImage))
                .Callback<Image>(v => imageList.Add(v));

            //Action
            await _addImage.Execute(testImage);

            //Result
            Assert.AreEqual(imageList.Count, 1);
        }
    }
}