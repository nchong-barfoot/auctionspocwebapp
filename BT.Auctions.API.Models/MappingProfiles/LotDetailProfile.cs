using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class LotDetailProfile : Profile
    {
        public LotDetailProfile()
        {
            CreateMap<LotDetailDto, LotDetail>()
                .ForMember(d => d.LotDetailId, o => o.Ignore())
                .ForMember(d => d.Key, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Key));
                    o.MapFrom(source => source.Key);
                })
                .ForMember(d => d.Value, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Value));
                    o.MapFrom(source => source.Value);
                })
                .ForMember(d => d.LotId, o =>
                {
                    o.PreCondition(source => source.LotId.HasValue);
                    o.MapFrom(source => source.LotId);
                })
                .ReverseMap()
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}