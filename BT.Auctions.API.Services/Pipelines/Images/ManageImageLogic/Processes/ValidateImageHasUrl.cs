using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic.Processes
{
    /// <summary>
    /// Process to Validate image Url
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Image}" />
    public class ValidateImageHasUrl : IProcess<Image>
    {
        public Task Execute(Image passObject)
        {
            if (string.IsNullOrWhiteSpace(passObject.Url))
            {
                passObject.CancellationReason = "Image URL is required";
                passObject.IsCancelled = true;
            }
            return Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}