using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;

namespace BT.Auctions.API.Data.Interfaces
{
    public interface IMediaRepository
    {
        /// <summary>
        /// Gets the media.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Media>> GetMedia();

        /// <summary>
        /// Adds the media.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <returns></returns>
        Task<Media> AddMedia(Media media);

        /// <summary>
        /// Updates the media.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <param name="media">The media.</param>
        /// <returns></returns>
        Task<Media> UpdateMedia(int mediaId, Media media);

        /// <summary>
        /// Deletes the media.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <returns></returns>
        Task DeleteMedia(int mediaId);

        /// <summary>
        /// Gets the media by identifier.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <returns></returns>
        Task<Media> GetMediaById(int mediaId);
    }
}