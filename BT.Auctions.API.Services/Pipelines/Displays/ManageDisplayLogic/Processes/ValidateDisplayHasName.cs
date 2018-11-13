using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Process to Validate Display Name
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Display}" />
    public class ValidateDisplayHasName : IProcess<Display>
    {
        public async Task Execute(Display passObject)
        {
           if(string.IsNullOrWhiteSpace(passObject.DisplayName))
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = "A Display Name is required";
            }
            await Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}