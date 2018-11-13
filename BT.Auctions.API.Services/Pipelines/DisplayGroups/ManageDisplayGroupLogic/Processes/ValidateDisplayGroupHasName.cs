using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Validate DisplayGroup Name
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayGroup}" />
    public class ValidateDisplayGroupHasName : IProcess<DisplayGroup>
    {
        public async Task Execute(DisplayGroup passObject)
        {
           if(string.IsNullOrWhiteSpace(passObject.Name))
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = "A Display Group Name is required";
            }
            await Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}