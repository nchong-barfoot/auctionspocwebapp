using System;
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
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;

namespace BT.Auctions.API.Tests.Services
{
    [TestClass]
    public class MediasServiceTests
    {
        private MediaService _mediasService;
        private Mock<ILogger<Media>> _logger;
        private Mock<IMediaRepository> _mediaRepository;
        private List<Media> _mediaList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Media>>();
            _mediaRepository = new Mock<IMediaRepository>();
            var serviceOptions = Options.Create(new ServiceSettings());
            serviceOptions.Value.StorageAccountName = "test";
            serviceOptions.Value.StorageAccessKey = "test";
            _mediaList = new List<Media>
            {
                new Media
                {
                    MediaId = 1,
                    Title = "Video Test"
                },
                new Media
                {
                    MediaId = 2,
                    Title = "Video Test"
                },
                new Media
                {
                    MediaId = 3,
                    Title = "Video Test"
                }
            };

            _mediasService = new MediaService(_logger.Object, _mediaRepository.Object, Mapper.Instance, serviceOptions);
        }

        [TestMethod]
        public async Task Get_Media_With_Valid_Media_Requested()
        {
            //Setup
            _mediaRepository.Setup(v => v.GetMedia())
                .Returns(Task.FromResult(_mediaList as IEnumerable<Media>));

            //Action
            var testMedias = await _mediasService.GetMedia();

            //Result
            Assert.AreEqual(3, testMedias.Count());
        }


        [TestMethod]
        public async Task Update_Media_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var mediaTitle = "testing";
            var mediaDto = new MediaDto {Title = mediaTitle };
            _mediaRepository.Setup(a => a.UpdateMedia(It.IsAny<int>(), It.IsAny<Media>()))
                .Returns((int id, Media media) => Task.FromResult(media))
                .Callback<int, Media>((i, v) => _mediaList.Single(li => li.MediaId == v.MediaId).Title = v.Title);
            _mediaRepository.Setup(a => a.GetMediaById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_mediaList.Single(b => b.MediaId == id)));

            //Action
            var mediaResult = await _mediasService.UpdateMedia(1, mediaDto);

            //Result
            Assert.AreEqual(mediaTitle, mediaResult.Title);
            Assert.IsFalse(mediaResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_Media_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var mediaTitle = "testing";
            var mediaDto = new MediaDto { Title = mediaTitle };
            _mediaRepository.Setup(a => a.GetMediaById(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(_mediaList.Single(b => b.MediaId == id)));

            //Action
            var mediaResult = await _mediasService.UpdateMedia(5, mediaDto);

            //Result
            Assert.IsTrue(mediaResult.IsCancelled);
        }

        [TestMethod]
        public async Task Add_Valid_Media_To_System_Media_Details_Provided()
        {
            //Setup
            var mediaToAddDto = new MediaDto
            {
                MediaId = 1,
                Title = "updated"
            };

            var media = new Media
            {
                MediaId = 1,
                Title = "current"
            };
            
            _mediaRepository.Setup(a => a.AddMedia(It.IsAny<Media>())).Returns(Task.FromResult(media));

            //Action
            var mediaAdded = await _mediasService.AddMedia(mediaToAddDto);

            //Result
            Assert.IsFalse(mediaAdded.IsCancelled);
        }

        [TestMethod]
        public async Task Delete_Media_Not_Found_In_Systems()
        {
            //Setup
            var mediaTitle = "testing";
            var mediaDto = new MediaDto { Title = mediaTitle };
            _mediaRepository.Setup(a => a.DeleteMedia(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(_mediaList.Remove(new Media {MediaId = id})));

            //Action
            var mediaResult = await _mediasService.DeleteMedia(1);

            //Result
            Assert.IsTrue(mediaResult.IsCancelled);
        }
    }
}
