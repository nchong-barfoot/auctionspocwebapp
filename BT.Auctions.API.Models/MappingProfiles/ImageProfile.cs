using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            AllowNullCollections = true;
            CreateMap<ImageDto, Image>()
                .ForMember(d => d.ImageId, o => o.Ignore())
                .ForMember(d => d.LotId, o =>
                {
                    o.PreCondition(source => source.LotId.HasValue);
                    o.MapFrom(source => source.LotId.Value);
                })
                .ForMember(d => d.Order, o =>
                {
                    o.PreCondition(source => source.Order.HasValue);
                    o.MapFrom(source => source.Order.Value);
                })
                .ForMember(d => d.Url, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Url));
                    o.MapFrom(source => source.Url);
                    
                })
                .ForMember(d => d.IsDisplayed, o =>
                {
                    o.Condition(source => source.IsDisplayed.HasValue);
                    o.MapFrom(source => source.IsDisplayed);

                })
                .ReverseMap()
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}