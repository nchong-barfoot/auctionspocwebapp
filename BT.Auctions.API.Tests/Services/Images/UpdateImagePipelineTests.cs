using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Images
{
    /// <summary>
    /// Update Image pipe line tests. These are used to test the entire end to end current pipeline
    /// </summary>
    [TestClass]
    public class UpdateImagePipelineTests
    {
        private UpdateImagePipeline _pipeline;
        private Mock<IImageRepository> _imageRepository;
        private Mock<ILotRepository> _lotRepository;
        private Mock<ILogger<Image>> _logger;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Image>>();
            _imageRepository = new Mock<IImageRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _pipeline = new UpdateImagePipeline(_logger.Object, _imageRepository.Object, _lotRepository.Object);
        }

        [TestMethod]
        public async Task Image_Update_Pipeline_Valid_Image()
        {
            //Setup
            var testImage = new Image
            {
                ImageId = 0,
                Url = "Updated URL"
            };
            var imageList = new List<Image>
            {
                new Image
                {
                    ImageId = 0,
                    Url = "URL To Be Updated"
                }
            };

            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()))
                .Returns((int id, Image image) => Task.FromResult(image))
                .Callback<int, Image>((i, v) => imageList.Single(li => li.ImageId == v.ImageId).Url = v.Url);
            _imageRepository.Setup(a => a.GetImageDetailsById(It.IsAny<int>()))
               .Returns(Task.FromResult(testImage));

            //Action
            var updatedImage = await _pipeline.Execute(testImage);

            //Result
            Assert.IsFalse(updatedImage.IsCancelled);
            Assert.AreEqual("Updated URL", imageList[0].Url);
        }

        [TestMethod]
        public async Task Image_Update_Pipeline_Invalid_Image_No_Address()
        {
            //Setup
            var testImage = new Image
            {
                ImageId = 0
            };
            var testLot = new Lot
            {
                LotId = 0
            };
            var imageList = new List<Image>
            {
                new Image
                {
                    ImageId = 1
                }
            };

            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()));
            _imageRepository.Setup(a => a.GetImageDetailsById(It.IsAny<int>()))
               .Returns(Task.FromResult(testImage));
            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>()))
                .Returns(Task.FromResult(testLot));

            //Action
            var updatedImage = await _pipeline.Execute(testImage);

            //Result
            Assert.AreEqual("Image URL is required",  updatedImage.CancellationReason);
        }
    }
}
