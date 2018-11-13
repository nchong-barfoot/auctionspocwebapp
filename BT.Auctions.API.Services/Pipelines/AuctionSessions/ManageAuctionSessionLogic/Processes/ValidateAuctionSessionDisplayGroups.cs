using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Models.Helpers;

namespace BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Process to Validate Auction Session Display Group exists and can be used
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{AuctionSession}" />
    public class ValidateAuctionSessionDisplayGroups : IProcess<AuctionSession>
    {
        private readonly IDisplayGroupRepository _displayGroupRepository;

        public ValidateAuctionSessionDisplayGroups(IDisplayGroupRepository displayGroupRepository)
        {
            _displayGroupRepository = displayGroupRepository;
        }

        public async Task Execute(AuctionSession passObject)
        {
            try
            {
                if(passObject.DisplayGroup != null)
                {
                    var displayGroup = await _displayGroupRepository.GetDisplayGroupById(passObject.DisplayGroup.DisplayGroupId);
                    if (displayGroup.AuctionSessions == null || !displayGroup.AuctionSessions.Any())
                    {
                        return;
                    }
                    foreach(var session in displayGroup.AuctionSessions)
                    {
                        if (session.AuctionSessionId == passObject.AuctionSessionId || !session.IsInSession || session.IsCancelled)
                            continue;

                        if(session.StartDate.HasValue && DateTimeHelper.CheckDateIsWithinRange(session.StartDate.Value, DateTime.UtcNow) && 
                           (session.FinishDate.HasValue && session.FinishDate.Value > DateTime.UtcNow || !session.FinishDate.HasValue)  && session.IsInSession)
                        {
                            passObject.IsCancelled = true;
                            passObject.CancellationReason = $"Display Group {displayGroup.DisplayGroupId} is currently being used by Auction Session {session.AuctionSessionId} at the same time.";
                            break;
                        }
                    }
                }
                
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Display Group was found matching ID {passObject.DisplayGroup.DisplayGroupId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}