using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using System.Linq;

namespace BT.Auctions.API.Models.MappingProfiles
{
    public class LotProfile : Profile
    {
        public LotProfile()
        {
            AllowNullCollections = true;
            CreateMap<LotDto, Lot>()
                .ForMember(d => d.LotId, o => o.Ignore())
                .ForMember(d => d.Bids, o => o.Ignore())
                .ForMember(d => d.AuctionSessionId, o =>
                {
                    o.PreCondition(source => source.AuctionSessionId.HasValue);
                    o.MapFrom(source => source.AuctionSessionId.Value);
                })
                .ForMember(d => d.Order, o =>
                {
                    o.PreCondition(source => source.Order.HasValue);
                    o.MapFrom(source => source.Order.Value);
                })
                .ForMember(d => d.ReserveMet, o =>
                {
                    o.PreCondition(source => source.ReserveMet.HasValue);
                    o.MapFrom(source => source.ReserveMet);
                })
                .ForMember(d => d.ListingId, o =>
                {
                    o.PreCondition(source => source.ListingId.HasValue);
                    o.MapFrom(source => source.ListingId.Value);
                })
                .ForMember(d => d.Address, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.Address));
                    o.MapFrom(source => source.Address);
                    
                })
                .ForMember(d => d.PostponedDateTime, o =>
                {
                    o.PreCondition(source => source.PostponedDateTime.HasValue);
                    o.MapFrom(source => source.PostponedDateTime.Value);
                })
                .ForMember(d => d.PlusGST, o =>
                {
                    o.PreCondition(source => source.PlusGST.HasValue);
                    o.MapFrom(source => source.PlusGST.Value);
                })
                .ForMember(d => d.AuctionStatus, o =>
                {
                    o.PreCondition(source => source.AuctionStatus.HasValue);
                    o.MapFrom(source => source.AuctionStatus.Value);
                })
                .ForMember(d => d.FeatureDescription, o =>
                {
                    o.Condition(source => !string.IsNullOrWhiteSpace(source.FeatureDescription));
                    o.MapFrom(source => source.FeatureDescription);
                })
                .ForMember(d => d.SoldDate, o =>
                {
                    o.MapFrom(source => source.SoldDate);
                })
                .ForMember(d => d.SoldPrice, o =>
                {
                    o.MapFrom(source => source.SoldPrice);
                })
                .ForMember(d => d.LotDetail, o =>
                {
                    o.PreCondition(source => source.LotDetail != null && source.LotDetail.Any());
                    o.MapFrom(source => source.LotDetail);
                })
                .ForMember(d => d.Images, o =>
                {
                    o.PreCondition(source => source.Images != null && source.Images.Any());
                    o.MapFrom(source => source.Images);
                })
                .ForMember(d => d.IsSalePriceHidden, o =>
                {
                    o.PreCondition(source => source.IsSalePriceHidden.HasValue);
                    o.MapFrom(source => source.IsSalePriceHidden.Value);
                })
                .ForMember(d => d.IsPaused, o =>
                {
                    o.PreCondition(source => source.IsPaused.HasValue);
                    o.MapFrom(source => source.IsPaused.Value);
                })
                .ReverseMap()
                .ForMember(d => d.IsSalePriceHidden, o =>
                {
                    o.MapFrom(source => source.IsSalePriceHidden.HasValue && source.IsSalePriceHidden.Value);
                })
                .ForMember(d => d.IsPaused, o =>
                {
                    o.MapFrom(source => source.IsPaused.HasValue && source.IsPaused.Value);
                })
                .ForMember(d => d.Bids, o =>
                {
                    o.PreCondition(source => source.Bids != null && source.Bids.Any());
                    o.MapFrom(source => source.Bids);
                })
                .ForAllOtherMembers(o => o.UseDestinationValue());
        }
    }
}