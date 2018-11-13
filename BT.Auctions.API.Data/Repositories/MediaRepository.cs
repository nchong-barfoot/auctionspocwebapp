using BT.Auctions.API.Data.Contexts;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Repositories
{
    /// <inheritdoc />
    /// <summary>
    /// Media repository to handle all database communications for media specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IMediaRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.Media" />
    public class MediaRepository : IMediaRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public MediaRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the media.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <returns>Media</returns>
        public async Task<Media> AddMedia(Media media)
        {
            media.CreatedOn = DateTime.UtcNow;
            var addedMedia = _auctionsContext.Medias.AddAsync(media);
            await _auctionsContext.SaveChangesAsync();
            return addedMedia.Result.Entity;
        }

        /// <summary>
        /// Gets the media.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Media> GetMediaById(int id)
        {
            return await _auctionsContext.Medias.SingleAsync(b => b.MediaId == id);
        }

        /// <summary>
        /// Gets the medias by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Media>> GetMedia()
        {
            return await _auctionsContext.Medias.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Updates the media.
        /// </summary>
        /// <param name="mediaId">the id of the media to update</param>
        /// <param name="media">The media.</param>
        /// <returns></returns>
        public async Task<Media> UpdateMedia(int mediaId, Media media)
        {
            media.MediaId = mediaId;
            media.ModifiedOn = DateTime.UtcNow;
            _auctionsContext.Entry(media).State = EntityState.Modified;
            await _auctionsContext.SaveChangesAsync();
            return media;
        }

        /// <summary>
        /// Deletes the media.
        /// </summary>
        /// <param name="mediaId">The media identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task DeleteMedia(int mediaId)
        {
            var deleteMedia = await _auctionsContext.Medias.SingleAsync(m => m.MediaId == mediaId);
            _auctionsContext.Entry(deleteMedia).State = EntityState.Deleted;
            await _auctionsContext.SaveChangesAsync();
        }
    }
}
