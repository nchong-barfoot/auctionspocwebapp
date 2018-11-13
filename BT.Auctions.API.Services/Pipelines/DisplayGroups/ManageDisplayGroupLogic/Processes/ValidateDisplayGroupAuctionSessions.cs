using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Validate display group auction sessions exist
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayGroup}" />
    public class ValidateDisplayGroupAuctionSessions : IProcess<DisplayGroup>
    {
        private readonly IAuctionSessionRepository _auctionSessionRepository;

        public ValidateDisplayGroupAuctionSessions(IAuctionSessionRepository auctionSessionRepository)
        {
            _auctionSessionRepository = auctionSessionRepository;
        }

        public async Task Execute(DisplayGroup passObject)
        {
            var currentSession = new AuctionSession();
            try
            {
                if (passObject.AuctionSessions == null)
                    return;
                var sessionsWithDates = new List<AuctionSession>();

                foreach (var session in passObject.AuctionSessions)
                {
                    currentSession = session;
                    var foundSession = await _auctionSessionRepository.GetAuctionSessionById(session.AuctionSessionId);
                    //Require users to update the auction session to allow new assignments of display groups. 
                    //Don't allow display groups to overwrite auction sessions
                    if(foundSession.DisplayGroupId.HasValue && foundSession.DisplayGroupId != passObject.DisplayGroupId)
                    {
                        passObject.IsCancelled = true;
                        passObject.CancellationReason = $"Auction Session {foundSession.AuctionSessionId} has already been assigned a display group." +
                            $" Please remove the display group from the auction session to proceed.";
                        break;
                    }
                    else if (foundSession.StartDate.HasValue && foundSession.FinishDate.HasValue)
                    {
                        sessionsWithDates.Add(foundSession);
                    }
                }

                if(!passObject.IsCancelled)
                {
                    //Do a final calculation on the sessions being added to compare against one another in the collection
                    //Only possible after we get the auction session information from the database.
                    //Handled here to stop consequent calls to the database to populate auction session
                    for (int i = 0; i < sessionsWithDates.Count - 1; i++)
                    {
                        for (int j = i + 1; j < sessionsWithDates.Count; j++)
                        {
                            //If start date 1 is within range of start and finish date 2 or finish date 1 is within the range of start and finish date 2
                            //this check will be executed the opposite way to ensure date 2 range does not conflict with date 1 range
                            if(DateTimeHelper.CheckDatesAreWithinRange(sessionsWithDates[i].StartDate.Value, sessionsWithDates[i].FinishDate.Value, 
                                    sessionsWithDates[j].StartDate.Value, sessionsWithDates[j].FinishDate.Value))
                                {
                                passObject.IsCancelled = true;
                                passObject.CancellationReason = $"Auction Session {sessionsWithDates[i].AuctionSessionId} occurs at the same time range as {sessionsWithDates[j].AuctionSessionId}. " +
                                    $"Please adjust the start and finish dates of one of these auction sessions.";
                                break;
                            }
                        }
                    }
                }

            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Auction Session was found matching ID {currentSession.AuctionSessionId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}