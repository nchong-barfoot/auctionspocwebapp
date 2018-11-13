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
    public class ImagesServiceTests
    {
        private ImageService _imagesService;
        private Mock<ILogger<Image>> _logger;
        private Mock<IImageRepository> _imageRepository;
        private Mock<ILotRepository> _lotRepository;
        private List<Image> _imageList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Image>>();
            _imageRepository = new Mock<IImageRepository>();
            _lotRepository = new Mock<ILotRepository>();
            _imageList = new List<Image>
            {
                new Image
                {
                    ImageId = 0,
                    Url = "testurl.com",
                    Order = 0,
                    LotId = 0
                },
                new Image
                {
                    ImageId = 1,
                    Url = "testurl.com",
                    Order = 0,
                    LotId = 0
                }
            };

            _imagesService = new ImageService(_logger.Object, _imageRepository.Object, _lotRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public async Task Get_Image_By_Auction_Session_Id_With_Valid_Image_Requested()
        {
            //Setup
            _imageRepository.Setup(v => v.GetImagesByLotId(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_imageList.Where(v => v.LotId == id)));

            //Action
            var testImages = await _imagesService.GetImagesByLotId(0);

            //Result
            Assert.AreEqual(2, testImages.Count());
        }

        [TestMethod]
        public async Task Get_All_Images_In_System()
        {
            //Setup
            var imageTask = Task.FromResult(_imageList as IEnumerable<Image>);
            _imageRepository.Setup(v => v.GetImages()).Returns(imageTask);

            //Action
            var imageListResult = await _imagesService.GetImages();

            //Result
            Assert.AreEqual(imageListResult.Count(), 2);
        }

        [TestMethod]
        public async Task Get_All_Images_Empty_Image_List_In_System()
        {
            //Setup
            var imageList = new List<Image>();
            var imageTask = Task.FromResult((IEnumerable<Image>)imageList);
            _imageRepository.Setup(v => v.GetImages()).Returns(imageTask);

            //Action
            var imageListResult = await _imagesService.GetImages();

            //Result
            Assert.AreEqual(imageListResult.Count(), 0);
        }

        [TestMethod]
        public async Task Update_Image_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var imageDto = new ImageDto {Url = "Test Image", LotId = 1};
            _imageRepository.Setup(v => v.GetImageDetailsById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_imageList.Single(v => v.ImageId == id)));
            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()))
                .Returns((int id, Image image) => Task.FromResult(image))
                .Callback<int, Image>((i, v) => _imageList.Single(li => li.ImageId == v.ImageId).Url = v.Url);

            //Action
            var imageResult = await _imagesService.UpdateImage(1, imageDto);

            //Result
            Assert.AreEqual(imageResult.Url, "Test Image");
            Assert.IsFalse(imageResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_Image_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var imageDto = new ImageDto { Url = "Test Image", ImageId = 999, LotId = 3 };
            _imageRepository.Setup(v => v.GetImageDetailsById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_imageList.Single(v => v.ImageId == id)));
            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()));

            //Action
            var imageResult = await _imagesService.UpdateImage(2, imageDto);

            //Result
            Assert.IsTrue(imageResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_Image_No_Lot_ID()
        {
            //Setup
            var imageDto = new ImageDto { Url = "Test Image", ImageId = 999 };
            _imageRepository.Setup(v => v.GetImageDetailsById(It.IsAny<int>()));
            _imageRepository.Setup(a => a.UpdateImage(It.IsAny<int>(), It.IsAny<Image>()));

            //Action
            var imageResult = await _imagesService.UpdateImage(2, imageDto);

            //Result
            Assert.IsTrue(imageResult.IsCancelled);
        }

        [TestMethod]
        public async Task Add_Invalid_Image_To_System_No_Image_Details_Provided()
        {
            //Setup
            var imageToAddDto = new ImageDto
            {

            };

            _imageRepository.Setup(a => a.AddImage(It.IsAny<Image>()));

            //Action
            var imageAdded = await _imagesService.AddImage(imageToAddDto);

            //Result
            Assert.IsTrue(imageAdded.IsCancelled);
            Assert.AreEqual(2, _imageList.Count);
        }

        [TestMethod]
        public async Task Add_Valid_Image_To_System_Image_Details_Provided()
        {
            //Setup
            var imageToAddDto = new ImageDto
            {
                Url = "Test Url",
                LotId = 3
            };
            var testLot = new Lot
            {
                LotId = 3
            };

            _lotRepository.Setup(a => a.GetLotById(It.IsAny<int>())).Returns(Task.FromResult(testLot));
            _imageRepository.Setup(a => a.AddImage(It.IsAny<Image>())).Returns(Task.FromResult(new Image()));

            //Action
            var imageAdded = await _imagesService.AddImage(imageToAddDto);

            //Result
            Assert.IsFalse(imageAdded.IsCancelled);
        }

        [TestMethod]
        public async Task Remove_Valid_Image_From_System()
        {
            //Setup
            var imageRemoveList = new List<Image>
            {
                new Image
                {
                    ImageId = 0
                }
            };

            _imageRepository.Setup(a => a.DeleteImage(It.IsAny<int>()))
                .Returns<int>(imageId => Task.FromResult(imageRemoveList.Single(i => i.ImageId == imageId)))
                .Callback<int>(imageId => imageRemoveList.RemoveAll(li => imageId == li.ImageId));

            //Action
            await _imagesService.DeleteImage(0);

            //Result
            Assert.AreEqual(0, imageRemoveList.Count);
        }

        [TestMethod]
        public async Task Remove_Invalid_Image_From_System()
        {
            //Setup
            var imageRemoveList = new List<Image>
            {
                new Image
                {
                    ImageId = 0
                }
            };

            _imageRepository.Setup(a => a.DeleteImage(It.IsAny<int>()))
                .Returns<int>(imageId => Task.FromResult(imageRemoveList.Single(i => i.ImageId == imageId)));

            //Action
            var result = await _imagesService.DeleteImage(1);

            //Result
            Assert.IsTrue(result.IsCancelled);
        }
    }
}
