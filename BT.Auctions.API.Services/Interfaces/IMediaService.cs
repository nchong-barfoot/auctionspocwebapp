using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using Microsoft.AspNetCore.Http;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IMediaService
    {
        /// <summary>
        /// Adds the media.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <returns></returns>
        Task<Media> AddMedia(MediaDto media);

        /// <summary>
        /// Gets the media.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Media>> GetMedia();

        /// <summary>
        /// Updates the media.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <param name="media">The media.</param>
        /// <returns></returns>
        Task<Media> UpdateMedia(int mediaId, MediaDto media);

        /// <summary>
        /// Deletes the media.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <returns></returns>
        Task<Media> DeleteMedia(int mediaId);
    }
}