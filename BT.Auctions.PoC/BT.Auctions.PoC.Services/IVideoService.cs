using BT.Auctions.Poc.Models;

namespace BT.Auctions.PoC.Services
{
    public interface IVideoService
    {
        /// <summary>
        /// Gets the video by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        Video GetVideoById(int Id);
    }
}