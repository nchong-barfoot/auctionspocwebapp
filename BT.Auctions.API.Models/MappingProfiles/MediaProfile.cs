using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class MediaProfile : Profile
    {
        public MediaProfile()
        {
            AllowNullCollections = true;
            CreateMap<MediaDto, Media>()
                .ForMember(d => d.MediaId, o => o.Ignore())
                .ForMember(d => d.CreatedOn, o => o.Ignore())
                .ForMember(d => d.Title, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Title));
                    o.MapFrom(source => source.Title);
                })
                .ForMember(d => d.Url, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Url));
                    o.MapFrom(source => source.Url);
                })
                .ForMember(d => d.FileName, o => o.UseDestinationValue())
                .ReverseMap()
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}