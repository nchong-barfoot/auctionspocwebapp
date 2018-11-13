using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Common.LogicPipelines
{
    public abstract class LogicPipeline<TObject>
        : HashSet<IProcess<TObject>>
        where TObject : ILogicPipelineObject
    {
        private static ILogger<TObject> _logger;

        /// <summary>
        /// Will create a logical pipeline for processing an object of <see cref="TObject"/>
        /// </summary>
        /// <param name="logger">Injected Logging service</param>
        protected LogicPipeline(ILogger<TObject> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Will process the object of <see cref="TObject"/> against all processes include in this pipeline
        /// </summary>
        /// <param name="passedObject">The object to be processed</param>
        /// <returns>The a processed <see cref="TObject"/></returns>
        public async Task<TObject> Execute(TObject passedObject)
        {
            foreach (var process in this)
                if (passedObject.IsCancelled == false)
                {
                    await process.Execute(passedObject);
                }
                else
                {
                    _logger.LogDebug(passedObject.CancellationReason);
                }
            return passedObject;
        }

        /// <summary>
        /// Will remove process <see cref="TObject"/> from Pipeline
        /// Not recommended.
        /// </summary>
        /// <param name="process"><see cref="TObject"/></param>
        /// <returns>true if removed</returns>
        public new bool Remove(IProcess<TObject> process)
        {
            _logger.LogWarning($"Process has been removed from {this.GetType().FullName}. This is not recommended as it'll affect all other processed items");
            return base.Remove(process);
        }
    }
}