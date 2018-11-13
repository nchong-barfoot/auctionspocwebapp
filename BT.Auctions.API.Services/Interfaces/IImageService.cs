using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>The Added Image</returns>
        Task<Image> AddImage(ImageDto image);

        /// <summary>
        /// Gets the image by identifier.
        /// </summary>
        /// <param name="lotId">The identifier.</param>
        /// <returns>Collection of Images</returns>
        Task<IEnumerable<Image>> GetImagesByLotId(int lotId);
        
        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <returns>Collection of Images</returns>
        Task<IEnumerable<Image>> GetImages();

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="image">The image.</param>
        /// <returns>The Updated Image</returns>
        Task<Image> UpdateImage(int imageId, ImageDto image);

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        Task<Image> DeleteImage(int imageId);
    }
}