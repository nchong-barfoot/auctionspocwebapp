using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic
{
    /// <summary>
    /// Update Display  Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Display}" />
    public class UpdateDisplayPipeline : LogicPipeline<Display>
    {
        public UpdateDisplayPipeline(ILogger<Display> logger, IDisplayRepository displayRepository, IVenueRepository venueRepository) : base(logger)
        {
            Add(new ValidateDisplayHasName());
            Add(new ValidateDisplayVenueExists(venueRepository));
            Add(new ValidateDisplayExists(displayRepository));
            Add(new UpdateDisplay(displayRepository));
        }
    }
}