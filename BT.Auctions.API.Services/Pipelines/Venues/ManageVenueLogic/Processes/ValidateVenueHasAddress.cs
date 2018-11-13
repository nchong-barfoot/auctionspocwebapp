using System.Threading.Tasks;
using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Process to Validate venue address. Deemed valid if suburb, region and street are all provided
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Venue}" />
    public class ValidateVenueHasAddress : IProcess<Venue>
    {
        public Task Execute(Venue passObject)
        {
            //venues require a street, suburb and region to have a valid address
            if (string.IsNullOrWhiteSpace(passObject.Street) || string.IsNullOrWhiteSpace(passObject.Suburb) || string.IsNullOrWhiteSpace(passObject.Region))
            {
                passObject.CancellationReason = "Venue's address requires a street, suburb and region";
                passObject.IsCancelled = true;
            }
            return Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}