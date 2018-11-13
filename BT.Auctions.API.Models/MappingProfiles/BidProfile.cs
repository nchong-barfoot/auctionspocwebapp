using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using System.Linq;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class BidProfile : Profile
    {
        public BidProfile()
        {
            AllowNullCollections = true;
            CreateMap<BidDto, Bid>()
                .ForMember(d => d.BidId, o => o.Ignore())
                .ForMember(d => d.IsRejected, o =>
                {
                    o.PreCondition(source => source.IsRejected.HasValue);
                    o.MapFrom(source => source.IsRejected);
                })
                .ForMember(d => d.Amount, o =>
                {
                    o.PreCondition(source => source.Amount.HasValue);
                    o.MapFrom(source => source.Amount);
                })
                .ForMember(d => d.LotId, o =>
                {
                    o.PreCondition(source => source.LotId.HasValue);
                    o.MapFrom(source => source.LotId);
                })
                .ForMember(d => d.ReserveMet, o =>
                {
                    o.PreCondition(source => source.ReserveMet.HasValue);
                    o.MapFrom(source => source.ReserveMet);
                })
                .ReverseMap()
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}