using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using System.Linq;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class DisplayGroupProfile : Profile
    {
        public DisplayGroupProfile()
        {
            AllowNullCollections = true;
            CreateMap<DisplayGroupDto, DisplayGroup>()
                .ForMember(d => d.DisplayGroupId, o => o.Ignore())
                .ForMember(d => d.VenueId, o =>
                {
                    o.PreCondition(source => source.VenueId.HasValue);
                    o.MapFrom(source => source.VenueId.Value);
                })
                .ForMember(d => d.Name, o =>
                {
                    o.PreCondition(source => !string.IsNullOrWhiteSpace(source.Name));
                    o.MapFrom(source => source.Name);
                })
                .ForMember(d => d.Description, o =>
                {
                    o.PreCondition(source => !string.IsNullOrWhiteSpace(source.Description));
                    o.MapFrom(source => source.Description);
                })
                .ForMember(d => d.AuctionSessions, o =>
                {
                    o.PreCondition(source => source.AuctionSessionIds != null);
                    o.MapFrom(source => source.AuctionSessionIds.Select(id => new AuctionSession
                    {
                        AuctionSessionId = id
                    }).ToList());
                })
                .ForMember(d => d.DisplayGroupConfigurations, o =>
                {
                    o.PreCondition(source => source.DisplayConfigurationIds != null);
                    o.MapFrom(source => source.DisplayConfigurationIds.Select(id => new DisplayGroupConfigurations
                    {
                        DisplayConfigurationId = id,
                        DisplayConfiguration = new DisplayConfiguration
                        {
                            DisplayConfigurationId = id
                        }
                    }).ToList());
                })
                .ForMember(d => d.IsInUse, o =>
                {
                    o.MapFrom(source => source.IsInUse);
                })
                //.ForMember(d => d.Venue, o =>
                //{
                //    o.PreCondition(source => source.Images != null && source.Images.Any());
                //    o.MapFrom(source => source.Images);
                //})
                .ReverseMap()
                .ForMember(d => d.DisplayConfigurationIds, o =>
                {
                    o.PreCondition(source => source.DisplayGroupConfigurations != null && source.DisplayGroupConfigurations.Any());
                    o.MapFrom(source => source.DisplayGroupConfigurations.Select(groupConfig =>
                        groupConfig.DisplayConfigurationId
                    ));
                })
                .ForMember(d => d.AuctionSessionIds, o =>
                {
                    o.PreCondition(source => source.AuctionSessions != null && source.AuctionSessions.Any());
                    o.MapFrom(source => source.AuctionSessions.Select(session => 
                        session.AuctionSessionId
                    ));
                })
                .ForMember(d => d.DisplayCount, o =>
                {
                    o.PreCondition(source =>
                        source.DisplayGroupConfigurations != null &&
                        source.DisplayGroupConfigurations.Any());
                    o.MapFrom(
                        source => source.DisplayGroupConfigurations.Select(g => g.DisplayConfiguration).Count()
                    );
                })
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}