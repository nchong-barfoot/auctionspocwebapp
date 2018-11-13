using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Process to Validate lot address
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class ValidateLotHasAddress : IProcess<Lot>
    {
        public Task Execute(Lot passObject)
        {
            //lots require an address
            if (string.IsNullOrWhiteSpace(passObject.Address))
            {
                passObject.CancellationReason = "Lot address is required";
                passObject.IsCancelled = true;
            }
            return Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}