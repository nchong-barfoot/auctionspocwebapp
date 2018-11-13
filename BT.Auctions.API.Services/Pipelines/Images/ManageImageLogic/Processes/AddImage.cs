using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic.Processes
{
    /// <summary>
    /// Process to Add Image to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Image}" />
    public class AddImage : IProcess<Image>
    {
        private readonly IImageRepository _imageRepository;

        public AddImage(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task Execute(Image passObject)
        {
            await _imageRepository.AddImage(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}