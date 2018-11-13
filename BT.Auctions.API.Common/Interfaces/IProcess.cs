using System.Threading.Tasks;

namespace BT.Auctions.API.Common.Interfaces
{
    public interface IProcess<in TObject> where TObject : ILogicPipelineObject
    {
        /// <summary>
        ///  Will process the passed in <see cref="TObject"/>
        /// </summary>
        /// <param name="passObject"><see cref="TObject"/> to be processed</param>
        Task Execute(TObject passObject);

        /// <summary>
        /// Will record the reason a pipeline was canceled in this process. 
        /// </summary>
        string CancellationMessage { get; set; }
    }
}