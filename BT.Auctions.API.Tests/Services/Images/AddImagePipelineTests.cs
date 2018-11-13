using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Images
{
    /// <summary>
    /// Add Image pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class AddImagePipelineTests
    {
        private AddImagePipeline _pipeline;
        private Mock<IImageRepository> _imageRepository;
        private Mock<ILotRepository> _lotRepository;
        private Mock<ILogger<Image>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Image>>();
            _imageRepository = new Mock<IImageRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _pipeline = new AddImagePipeline(_logger.Object, _imageRepository.Object, _lotRepository.Object);
        }

        [TestMethod]
        public async Task Image_Add_Pipeline_Valid_Image()
        {
            //Setup
            var testImage = new Image
            {
                ImageId = 0,
                Url = "Test URL"
            };
            var imageList = new List<Image>();

            _imageRepository.Setup(a => a.AddImage(It.IsAny<Image>()))
                .Returns(Task.FromResult(testImage))
                .Callback<Image>(v => imageList.Add(v));

            //Action
            var addedImage = await _pipeline.Execute(testImage);

            //Result
            Assert.IsFalse(addedImage.IsCancelled);
            Assert.AreEqual(1, imageList.Count);
        }

        [TestMethod]
        public async Task Image_Add_Pipeline_Invalid_Image_No_Address()
        {
            //Setup
            var testImage = new Image
            {
                ImageId = 0
            };

            _imageRepository.Setup(a => a.AddImage(It.IsAny<Image>()))
                .Returns(Task.FromResult(testImage));

            //Action
            var addedImage = await _pipeline.Execute(testImage);

            //Result
            Assert.IsTrue(addedImage.IsCancelled);
            Assert.AreEqual(addedImage.CancellationReason, "Image URL is required");
        }

        [TestMethod]
        public async Task Image_Add_Pipeline_Invalid_Image_Null_Values()
        {
            //Setup
            var imageList = new List<Image>();

            _imageRepository.Setup(a => a.AddImage(It.IsAny<Image>()))
                .Returns(Task.FromResult((Image) null));

            //Action
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _pipeline.Execute(null));

            //Result
            Assert.IsTrue(imageList.Count == 0);
        }
    }
}