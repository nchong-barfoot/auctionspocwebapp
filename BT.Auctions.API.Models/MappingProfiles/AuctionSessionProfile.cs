using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using System.Linq;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class AuctionSessionProfile : Profile
    {
        public AuctionSessionProfile()
        {
            CreateMap<AuctionSessionDto, AuctionSession>()
                .ForMember(d => d.AuctionSessionId, o => o.Ignore())
                .ForMember(d => d.Lots, o => o.Ignore())
                .ForMember(d => d.Name, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Name));
                    o.MapFrom(source => source.Name);
                })
                .ForMember(d => d.AuctionSessionAdmin, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.AuctionSessionAdmin));
                    o.MapFrom(source => source.AuctionSessionAdmin);
                })
                .ForMember(d => d.IsInSession, o =>
                {
                    o.PreCondition(source => source.IsInSession.HasValue);
                    o.MapFrom(source => source.IsInSession.Value);
                })
                .ForMember(d => d.StartDate, o =>
                {
                    o.PreCondition(source => source.StartDate.HasValue);
                    o.MapFrom(source => source.StartDate.Value.ToUniversalTime());
                })
                .ForMember(d => d.FinishDate, o =>
                {
                    o.PreCondition(source => source.FinishDate.HasValue);
                    o.MapFrom(source => source.FinishDate.Value.ToUniversalTime());
                })
                .ForMember(d => d.VenueId, o =>
                {
                    o.PreCondition(source => source.VenueId.HasValue);
                    o.MapFrom(source => source.VenueId);
                })
                .ForMember(d => d.DisplayGroup, o =>
                {
                    o.PreCondition(source => source.DisplayGroupId.HasValue);
                    o.MapFrom(source => new DisplayGroup
                    {
                        DisplayGroupId = source.DisplayGroupId.Value
                    });
                })
                .ForMember(d => d.Lots, o =>
                {
                    o.PreCondition(source => source.Lots != null && source.Lots.Any());
                    o.MapFrom(source => source.Lots);
                })
                .ReverseMap()
                .ForMember(d => d.LotCount, o =>
                        o.MapFrom(source => source.Lots != null ? source.Lots.Count() : 0)
                 )
                .ForMember(d => d.DisplayGroupName, o =>
                {
                    o.PreCondition(source => source.DisplayGroup != null && !string.IsNullOrWhiteSpace(source.DisplayGroup.Name));
                    o.MapFrom(source => source.DisplayGroup.Name);
                })
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}