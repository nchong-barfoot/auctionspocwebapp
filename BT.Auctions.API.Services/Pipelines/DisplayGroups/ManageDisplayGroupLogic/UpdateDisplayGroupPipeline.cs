using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic
{
    /// <summary>
    /// Update DisplayGroup Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{DisplayGroup}" />
    public class UpdateDisplayGroupPipeline : LogicPipeline<DisplayGroup>
    {
        public UpdateDisplayGroupPipeline(ILogger<DisplayGroup> logger, IDisplayGroupRepository displayRepository, IDisplayConfigurationRepository displayConfigurationRepository, IAuctionSessionRepository auctionSessionRepository, IVenueRepository venueRepository) : base(logger)
        {
            Add(new ValidateDisplayGroupHasName());
            Add(new ValidateDisplayGroupDisplayConfigurations(displayConfigurationRepository));
            Add(new ValidateDisplayGroupExists(displayRepository));
            Add(new ValidateDisplayGroupVenueExists(venueRepository));
            Add(new ValidateDisplayGroupAuctionSessions(auctionSessionRepository));
            Add(new UpdateDisplayGroup(displayRepository));
        }
    }
}