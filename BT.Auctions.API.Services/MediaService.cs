using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services.Helpers;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// Media Service to handle logic and pipeline execution for Media endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IMediaService" />
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Media> _logger;
        private readonly StorageHelper _storageHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediaRepository">The auctions repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="serviceSettings">The service settings.</param>
        public MediaService(ILogger<Media> logger, IMediaRepository mediaRepository, IMapper mapper, IOptions<ServiceSettings> serviceSettings)
        {
            _mediaRepository = mediaRepository;
            _mapper = mapper;
            _logger = logger;
            _storageHelper = new StorageHelper(serviceSettings);
        }

        /// <summary>
        /// Posts the media.
        /// </summary>
        /// <param name="sourceMedia">The media.</param>
        /// <returns></returns>
        public async Task<Media> AddMedia(MediaDto sourceMedia)
        {
            await AddUpdateDuplicateMedia(sourceMedia);
            var mappedMedia = _mapper.Map<MediaDto, Media>(sourceMedia);
            return await _mediaRepository.AddMedia(mappedMedia);
        }

        /// <summary>
        /// Adds the update duplicate media. Shared method for handling duplicate file names into the blob storage
        /// </summary>
        /// <param name="media">The media.</param>
        /// <returns></returns>
        private async Task AddUpdateDuplicateMedia(MediaDto media)
        {
            if (media.Data != null)
            {
                var exists = await _storageHelper.CheckMediaExistsInBlobStorage(media.Data.FileName);
                var mediaName = "";
                if (exists)
                {
                    mediaName = $"{DateTime.UtcNow.Ticks}-{media.Data.FileName}";
                }
                await _storageHelper.UploadMediaToBlobStorage(media, mediaName);
            }
        }

        /// <summary>
        /// Gets the media
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Media>> GetMedia()
        {
            return await _mediaRepository.GetMedia();
        }

        /// <summary>
        /// Updates the media.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <param name="sourceMedia">The source media.</param>
        /// <returns></returns>
        public async Task<Media> UpdateMedia(int mediaId, MediaDto sourceMedia)
        {
            try
            {
                var uploadedMedia = await _mediaRepository.GetMediaById(mediaId);
                await AddUpdateDuplicateMedia(sourceMedia);
                var mappedMedia = _mapper.Map(sourceMedia, uploadedMedia);

                await _mediaRepository.UpdateMedia(mediaId, mappedMedia);
                return await _mediaRepository.GetMediaById(mediaId);
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new Media()
                {
                    CancellationReason = $"Could not find Media {mediaId} in System",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Deletes the media.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <returns></returns>
        public async Task<Media> DeleteMedia(int mediaId)
        {
            var resultingMedia = new Media();
            try
            {
                var mediaToDelete = await _mediaRepository.GetMediaById(mediaId);
                await _storageHelper.DeleteMediaFromBlobStorage(mediaToDelete.FileName);
                await _mediaRepository.DeleteMedia(mediaId);
            }
            catch (Exception ex)
            {
                //delete failed, return a failed delete
                resultingMedia.CancellationReason = "Failed to delete media. ";
                resultingMedia.IsCancelled = true;
                _logger.LogError(ex, "Failed to delete media.");
            }
            return resultingMedia;
        }
    }
}