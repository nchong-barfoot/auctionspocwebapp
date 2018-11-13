using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class DisplayProfile : Profile
    {
        public DisplayProfile()
        {
            CreateMap<DisplayDto, Display>()
                .ForMember(d => d.DisplayId, o => o.Ignore())
                .ForMember(d => d.DisplayName, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.DisplayName));
                    o.MapFrom(source => source.DisplayName);
                })
                .ForMember(d => d.VenueId, o =>
                {
                    o.PreCondition(source => source.VenueId.HasValue);
                    o.MapFrom(source => source.VenueId.Value);
                })
                .ForMember(d => d.Venue, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.VenueName, o =>
                {
                    o.PreCondition(source => source.Venue != null);
                    o.MapFrom(source => source.Venue.Name);
                })
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}