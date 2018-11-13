using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class VenueProfile : Profile
    {
        public VenueProfile()
        {
            CreateMap<VenueDto, Venue>()
                .ForMember(d => d.VenueId, o => o.Ignore())
                .ForMember(d => d.Name, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Name));
                    o.MapFrom(source => source.Name);
                })
                .ForMember(d => d.Region, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Region));
                    o.MapFrom(source => source.Region);
                })
                .ForMember(d => d.Street, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Street));
                    o.MapFrom(source => source.Street);
                })
                .ForMember(d => d.Suburb, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Suburb));
                    o.MapFrom(source => source.Suburb);
                })
                .ForMember(d => d.OnSite, o =>
                {
                    o.PreCondition(source => source.OnSite != null);
                    o.MapFrom(source => source.OnSite.Value);
                })
                .ReverseMap()
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}