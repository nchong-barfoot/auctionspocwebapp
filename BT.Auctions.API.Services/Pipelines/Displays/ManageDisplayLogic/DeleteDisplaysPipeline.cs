using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic
{
    /// <summary>
    /// Delete Display Pipeline. Has the default process flow constructed here
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Display}" />
    public class DeleteDisplayPipeline : LogicPipeline<Display>
    {
        public DeleteDisplayPipeline(ILogger<Display> logger, IDisplayRepository displayRepository) : base(logger)
        {
            Add(new ValidateDisplayExists(displayRepository));
            Add(new DeleteDisplay(displayRepository));
        }
    }
}