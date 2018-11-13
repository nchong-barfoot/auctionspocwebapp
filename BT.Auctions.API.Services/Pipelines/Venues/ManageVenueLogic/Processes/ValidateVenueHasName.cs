using System.Threading.Tasks;
using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Process to validate the venue has a valid name. Name is valid if not null or empty
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Venue}" />
    public class ValidateVenueHasName : IProcess<Venue>
    {
        public Task Execute(Venue passObject)
        {
            if (string.IsNullOrWhiteSpace(passObject.Name))
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = "Venue Name was not provided";
            }
            return Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}