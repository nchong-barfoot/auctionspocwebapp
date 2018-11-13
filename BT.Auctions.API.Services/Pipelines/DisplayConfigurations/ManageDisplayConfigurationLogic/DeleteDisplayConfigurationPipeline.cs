using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic
{
    /// <summary>
    /// Delete DisplayConfiguration Pipeline. Has the default process flow constructed here
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{DisplayConfiguration}" />
    public class DeleteDisplayConfigurationPipeline : LogicPipeline<DisplayConfiguration>
    {
        public DeleteDisplayConfigurationPipeline(ILogger<DisplayConfiguration> logger, IDisplayConfigurationRepository displayConfigurationRepository) : base(logger)
        {
            Add(new ValidateDisplayConfigurationExists(displayConfigurationRepository));
            Add(new DeleteDisplayConfiguration(displayConfigurationRepository));
        }
    }
}