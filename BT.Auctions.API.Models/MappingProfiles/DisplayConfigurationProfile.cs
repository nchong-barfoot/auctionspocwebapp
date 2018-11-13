using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using System.Linq;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class DisplayConfigurationProfile : Profile
    {
        public DisplayConfigurationProfile()
        {
            AllowNullCollections = true;
            CreateMap<DisplayConfigurationDto, DisplayConfiguration>()
                .ForMember(d => d.DisplayConfigurationId, o => o.Ignore())
                .ForMember(d => d.PlayVideo, o =>
                {
                    o.PreCondition(source => source.PlayVideo.HasValue);
                    o.MapFrom(source => source.PlayVideo);
                })
                .ForMember(d => d.PlayAudio, o =>
                {
                    o.PreCondition(source => source.PlayAudio.HasValue);
                    o.MapFrom(source => source.PlayAudio);
                })
                .ForMember(d => d.IsActive, o =>
                {
                    o.PreCondition(source => source.IsActive.HasValue);
                    o.MapFrom(source => source.IsActive.Value);
                })
                .ForMember(d => d.DisplayMode, o =>
                {
                    o.PreCondition(source => source.DisplayMode.HasValue);
                    o.MapFrom(source => source.DisplayMode.Value);
                })
                .ForMember(d => d.DisplayId, o =>
                {
                    o.PreCondition(source => source.DisplayId.HasValue);
                    o.MapFrom(source => source.DisplayId.Value);
                })
                .ForMember(d => d.DisplayGroupConfigurations, o =>
                {
                    o.PreCondition(source => source.DisplayGroupIds != null);
                    o.MapFrom(source => source.DisplayGroupIds.Select(id => new DisplayGroupConfigurations
                    {
                        DisplayGroupId = id
                    }).ToList());
                })
                .ReverseMap()
                .ForMember(d => d.DisplayGroupIds, o =>
                {
                    o.PreCondition(source => source.DisplayGroupConfigurations != null && source.DisplayGroupConfigurations.Any());
                    o.MapFrom(source => source.DisplayGroupConfigurations.Select(groupConfig =>
                        groupConfig.DisplayGroupId
                    ));
                })
                .ForMember(d => d.DisplayName, o =>
                {
                    o.PreCondition(source => source.Display != null);
                    o.MapFrom(source => source.Display.DisplayName);
                })
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}