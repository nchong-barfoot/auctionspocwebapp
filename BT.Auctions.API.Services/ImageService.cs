using System;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// Image Service to handle logic and pipeline execution for Image endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IImageService" />
    public class ImageService : IImageService
    {
        private readonly AddImagePipeline _addImageLogicPipeline;
        private readonly UpdateImagePipeline _updateImageLogicPipeline;

        private readonly IImageRepository _imageRepository;
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="imageRepository">The image repository.</param>
        /// <param name="lotRepository">The lot repository.</param>
        /// <param name="mapper">The mapper.</param>
        public ImageService(ILogger<Image> logger, IImageRepository imageRepository, ILotRepository lotRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _lotRepository = lotRepository;
            _mapper = mapper;
            _updateImageLogicPipeline = new UpdateImagePipeline(logger, _imageRepository, _lotRepository);
            _addImageLogicPipeline = new AddImagePipeline(logger, _imageRepository, _lotRepository);
        }

        /// <summary>
        /// Posts the image.
        /// </summary>
        /// <param name="sourceImage">The image.</param>
        /// <returns></returns>
        public async Task<Image> AddImage(ImageDto sourceImage)
        {
            //Listing ID to be validated before mapping and pipeline as int? will default to int and allow for a valid 0 result
            if(!sourceImage.LotId.HasValue)
                return await Task.FromResult(new Image()
                {
                    CancellationReason = $"A Lot ID is required",
                    IsCancelled = true
                });
            var mappedImage = _mapper.Map<ImageDto, Image>(sourceImage);
            return await _addImageLogicPipeline.Execute(mappedImage);
        }

        /// <summary>
        /// Gets the image by auction session identifier.
        /// </summary>
        /// <param name="lotId">The identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Image>> GetImagesByLotId(int lotId)
        {
            return await _imageRepository.GetImagesByLotId(lotId);
        }

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Image>> GetImages()
        {
            return await _imageRepository.GetImages();
        }

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="sourceImage">The source image.</param>
        /// <returns></returns>
        public async Task<Image> UpdateImage(int imageId, ImageDto sourceImage)
        {
            Image mappedImage = new Image();
            try
            {
                if (!sourceImage.LotId.HasValue)
                {
                    mappedImage.CancellationReason = $"A Lot ID is required";
                    mappedImage.IsCancelled = true;
                }
                else
                {
                    mappedImage = _mapper.Map(sourceImage, await _imageRepository.GetImageDetailsById(imageId));
                }
            }
            catch (InvalidOperationException)
            {
                mappedImage.CancellationReason = $"Could not find Image {imageId} in System";
                mappedImage.IsCancelled = true;
            }

            if (mappedImage.IsCancelled)
                return await Task.FromResult(mappedImage);
            return await _updateImageLogicPipeline.Execute(mappedImage);
        }

        /// <summary>
        /// Deletes a <see cref="Image"/>
        /// </summary>
        /// <param name="id">The identifier.</param>
        public async Task<Image> DeleteImage(int imageId)
        {
            var image = new Image();
            try
            {
                await _imageRepository.DeleteImage(imageId);
            }
            catch(InvalidOperationException)
            {
                image.CancellationReason = $"Invalid Request. Could not find Image {imageId} in System";
                image.IsCancelled = true;
            }
            return await Task.FromResult(image);
        }
    }
}