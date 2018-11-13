using BT.Auctions.API.Data.Contexts;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Repositories
{
    /// <inheritdoc />
    /// <summary>
    /// Image repository to handle all database communications for image specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IImageRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.Image" />
    public class ImageRepository : IImageRepository
    {
        private readonly AuctionsContext _auctionsContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRepository"/> class.
        /// </summary>
        /// <param name="auctionsContext">The auctions context.</param>
        public ImageRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// Image
        /// </returns>
        public async Task<Image> AddImage(Image image)
        {
            var addedImage = _auctionsContext.Images.AddAsync(image);
            await _auctionsContext.SaveChangesAsync();
            return addedImage.Result.Entity;
        }

        /// <summary>
        /// Gets the image by identifier.
        /// </summary>
        /// <param name="imageId">The identifier.</param>
        /// <returns>
        /// Image
        /// </returns>
        public async Task<Image> GetImageById(int imageId)
        {
            return await _auctionsContext.Images.AsNoTracking().Include(i => i.Lot).SingleAsync(v => v.ImageId == imageId);
        }

        /// <summary>
        /// Gets all the image details including joined entities by identifier.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns>
        /// Image
        /// </returns>
        public async Task<Image> GetImageDetailsById(int imageId)
        {
            return await _auctionsContext.Images.Include(i => i.Lot)
                .ThenInclude(l => l.AuctionSession)
                .SingleAsync(v => v.ImageId == imageId);
        }

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <returns>
        /// List of Images
        /// </returns>
        public async Task<IEnumerable<Image>> GetImages()
        {
            return await _auctionsContext.Images.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Gets the images by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Image>> GetImagesByLotId(int lotId)
        {
            return await _auctionsContext.Images.AsNoTracking()
                .Where(l => l.LotId == lotId).ToListAsync();
        }

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="imageId">the id of the image to update</param>
        /// <param name="image">The image.</param>
        /// <returns>The Updated Image</returns>
        public async Task<Image> UpdateImage(int imageId, Image image)
        {
            image.Lot = _auctionsContext.Lots.FirstOrDefault(a => a.LotId == image.LotId);
            _auctionsContext.Entry(image).State = EntityState.Modified;
            await _auctionsContext.SaveChangesAsync();
            return image;
        }

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public async Task DeleteImage(int imageId)
        {
            var imageToDelete = _auctionsContext.Images.Single(i => i.ImageId == imageId);
            _auctionsContext.Entry(imageToDelete).State = EntityState.Deleted;
            await _auctionsContext.SaveChangesAsync();
        }
    }
}
