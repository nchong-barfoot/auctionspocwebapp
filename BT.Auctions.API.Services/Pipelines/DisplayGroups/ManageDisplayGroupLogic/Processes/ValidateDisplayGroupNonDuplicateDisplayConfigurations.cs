using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Validate Display Groups have unique display configurations
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayGroup}" />
    public class ValidateDisplayGroupNonDuplicateDisplayConfigurations : IProcess<DisplayGroup>
    {
        public async Task Execute(DisplayGroup passObject)
        {
            if(passObject.DisplayGroupConfigurations != null && passObject.DisplayGroupConfigurations.GroupBy(config => config.DisplayConfigurationId).Any(d => d.Count() > 1))
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = "Duplicate display configurations are not allowed in a single display group";
            }
            await Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}