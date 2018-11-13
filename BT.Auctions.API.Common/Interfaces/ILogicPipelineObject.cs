namespace BT.Auctions.API.Common.Interfaces
{
    public interface ILogicPipelineObject
    {
        /// <summary>
        /// If the processing pipeline failed or canceled 
        /// </summary>
        bool IsCancelled { get; set; }

        /// <summary>
        /// Reason for the logic pipeline failure to process this object.
        /// </summary>
        string CancellationReason { get; set; }
    }
}