using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic
{
    /// <summary>
    /// Update Display Configuration Pipeline
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{DisplayConfiguration}" />
    public class AddDisplayConfigurationPipeline : LogicPipeline<DisplayConfiguration>
    {
        public AddDisplayConfigurationPipeline(ILogger<DisplayConfiguration> logger, IDisplayConfigurationRepository displayConfigurationRepository, IDisplayRepository displayRepository, IDisplayGroupRepository displayGroupRepository) : base(logger)
        {
            Add(new ValidateDisplayConfigurationDisplayExists(displayRepository));
            Add(new ValidateDisplayConfigurationDisplayGroupExists(displayGroupRepository));
            Add(new AddDisplayConfiguration(displayConfigurationRepository));
        }
    }
}