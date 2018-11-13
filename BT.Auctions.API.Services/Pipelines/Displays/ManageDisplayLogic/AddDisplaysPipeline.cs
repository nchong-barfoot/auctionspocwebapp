using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic
{
    /// <summary>
    /// Add/Register new Display Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Display}" />
    public class AddDisplayPipeline : LogicPipeline<Display>
    {
        public AddDisplayPipeline(ILogger<Display> logger, IDisplayRepository displayRepository, IVenueRepository venueRepository) : base(logger)
        {
            Add(new ValidateDisplayHasName());
            Add(new ValidateDisplayVenueExists(venueRepository));
            Add(new AddDisplay(displayRepository));
        }
    }
}