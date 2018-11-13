using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic
{
    /// <summary>
    /// Delete DisplayGroup Pipeline. Has the default process flow constructed here
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{DisplayGroup}" />
    public class DeleteDisplayGroupPipeline : LogicPipeline<DisplayGroup>
    {
        public DeleteDisplayGroupPipeline(ILogger<DisplayGroup> logger, IDisplayGroupRepository displayRepository) : base(logger)
        {
            Add(new ValidateDisplayGroupExists(displayRepository));
            Add(new DeleteDisplayGroup(displayRepository));
        }
    }
}