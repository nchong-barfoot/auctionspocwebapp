using System;
using System.Threading.Tasks;
using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Process to Validate Auction Session already exists. Deemed valid if a auctionSession matching the id is returned from the db
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class ValidateLotAuctionSessionExists : IProcess<Lot>
    {
        private readonly IAuctionSessionRepository _auctionSessionRepository;

        public ValidateLotAuctionSessionExists(IAuctionSessionRepository auctionSessionRepository)
        {
            _auctionSessionRepository = auctionSessionRepository;
        }

        public async Task Execute(Lot passObject)
        {
            try
            {
                await _auctionSessionRepository.GetAuctionSessionById(passObject.AuctionSessionId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Auction Session was found matching ID {passObject.AuctionSessionId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}