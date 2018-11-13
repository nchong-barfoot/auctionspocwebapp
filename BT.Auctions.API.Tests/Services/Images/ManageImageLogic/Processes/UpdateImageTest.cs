using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic.Processes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services.Images.ManageImageLogic.Processes
{
    /// <summary>
    /// Update Image pipeline process testing
    /// </summary>
    [TestClass]
    public class UpdateImageTest 
    {
        private UpdateImage _updateImage;
        private Mock<IImageRepository> _imageRepository;

        [TestInitialize]
        public void Initialize()
        {
            _imageRepository = new Mock<IImageRepository>();
            _updateImage = new UpdateImage(_imageRepository.Object);
        }


        [TestMethod]
        public async Task Image_Update_Valid_Image()
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
                    Url = "Test URL"
                }
            };

            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()))
                .Returns((int id, Image image) => Task.FromResult(image))
                .Callback<int, Image>((i, v) => imageList.Single(li => li.ImageId == v.ImageId).Url = v.Url);

            //Action
            await _updateImage.Execute(testImage);

            //Result
            Assert.AreEqual(imageList.ElementAt(0).Url, "Updated URL");
        }

        [TestMethod]
        public async Task Image_Update_Invalid_Image()
        {
            //Setup
            var testImage = new Image
            {
                ImageId = 1,
                Url = "Updated URL"
            };

            var imageList = new List<Image>
            {
                new Image
                {
                    ImageId = 0,
                    Url = "Test URL"
                }
            };

            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()))
                .Returns((int id, Image image) => Task.FromResult(image))
                .Callback<int, Image>((i, v) => imageList.Single(li => li.ImageId == v.ImageId).Url = v.Url);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateImage.Execute(testImage));
        }

        [TestMethod]
        public async Task Image_Update_Invalid_Image_No_Images()
        {
            //Setup
            var testImage = new Image
            {
                ImageId = 0,
                Url = "Updated URL"
            };

            var imageList = new List<Image>();

            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()))
                .Returns((int id, Image image) => Task.FromResult(image))
                .Callback<int, Image>((i, v) => imageList.Single(li => li.ImageId == v.ImageId).Url = v.Url);

            //Action
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _updateImage.Execute(testImage));
        }

        [TestMethod]
        public async Task Image_Update_No_Changes()
        {
            //Setup
            var testImage = new Image
            {
                ImageId = 0,
                Url = "Test URL"
            };

            var imageList = new List<Image>
            {
                new Image
                {
                    ImageId = 0,
                    Url = "Test URL"
                }
            };

            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()))
                .Returns((int id, Image image) => Task.FromResult(image))
                .Callback<int, Image>((i, v) => imageList.Single(li => li.ImageId == v.ImageId).Url = v.Url);

            //Action
            await _updateImage.Execute(testImage);

            //Result
            Assert.AreEqual(imageList.ElementAt(0).Url, "Test URL");
        }
    }
}