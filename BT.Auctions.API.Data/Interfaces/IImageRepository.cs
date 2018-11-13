using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// Image Repository Interface
    /// </summary>
    public interface IImageRepository
    {
        Task<Image> AddImage(Image image);
        Task<Image> GetImageById(int imageId);
        Task<Image> GetImageDetailsById(int imageId);
        Task<IEnumerable<Image>> GetImages();
        Task<IEnumerable<Image>> GetImagesByLotId(int lotId);
        Task<Image> UpdateImage(int imageId, Image image);
        Task DeleteImage(int imageId);
    }
}
