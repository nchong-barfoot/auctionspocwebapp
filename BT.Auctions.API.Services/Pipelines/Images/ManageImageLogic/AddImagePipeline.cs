using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic
{
    /// <summary>
    /// Update Image Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Image}" />
    public class AddImagePipeline : LogicPipeline<Image>
    {
        public AddImagePipeline(ILogger<Image> logger, IImageRepository imageRepository, ILotRepository lotRepository) : base(logger)
        {
            Add(new ValidateImageHasUrl());
            Add(new ValidateImageLotExists(lotRepository));
            Add(new AddImage(imageRepository));
        }
    }
}