using AutoMapper;

namespace RBC.BrokeragePlatform.Application.Profiles
{
    public class AccountMappingProfile: Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Domain.Entities.Account, SharedCore.DTOs.AccountDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
                .ForMember(dest => dest.CashBalance, opt => opt.MapFrom(src => src.CashBalance))
                .ReverseMap();

            CreateMap<Domain.Entities.Position, SharedCore.DTOs.PositionDto>()
                .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.PositionId))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.EquityId, opt => opt.MapFrom(src => src.EquityId))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.AverageCostPerShare, opt => opt.MapFrom(src => src.AverageCostPerShare))                
                .ReverseMap();
        }
    }
}
