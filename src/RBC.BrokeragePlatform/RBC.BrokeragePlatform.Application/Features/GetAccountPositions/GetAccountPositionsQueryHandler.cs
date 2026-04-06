using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Features.GetAccountPositions
{
    public class GetAccountPositionsQueryHandler : IRequestHandler<GetAccountPositionsQuery, IEnumerable<PositionDto>>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IEquityRepository _equityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountPositionsQueryHandler> _logger;

        public GetAccountPositionsQueryHandler(IPositionRepository positionRepository, IEquityRepository equityRepository, IMapper mapper, ILogger<GetAccountPositionsQueryHandler> logger)
        {
            _positionRepository = positionRepository;
            _equityRepository = equityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PositionDto>> Handle(GetAccountPositionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAccountPositionsQuery started - Account Id: {AccountId}", request.AccountId);

            var positionDtos = Enumerable.Empty<PositionDto>();

            try
            {
                var positions = await _positionRepository.GetPositionsByAccountIdAsync(request.AccountId, cancellationToken);

                if (!positions.Any()) 
                { 
                    _logger.LogInformation("No positions found for Account Id: {AccountId}", request.AccountId);
                    return positionDtos;
                }

                positionDtos = _mapper.Map<IEnumerable<PositionDto>>(positions);

                // get current price for each position and update the DTOs
                var equityIds = positions.Select(p => p.EquityId).Distinct().ToList();

                var equities = await _equityRepository.GetEquitiesByIdsAsync(equityIds, cancellationToken);

                // update the position DTOs with current price and current value
                foreach (var positionDto in positionDtos)
                {
                    var equity = equities.FirstOrDefault(e => e.EquityId == positionDto.EquityId);
                    if (equity == null)
                    {
                        _logger.LogWarning("Equity not found for Equity Id: {EquityId} in Account Id: {AccountId}", positionDto.EquityId, request.AccountId);
                        continue;
                    }
                    positionDto.CurrentPrice =  equity.CurrentPrice;
                    positionDto.CurrentValue =  equity.CurrentPrice * positionDto.Quantity;
                }

                    _logger.LogInformation("Handling GetAccountPositionsQuery completed successfully - Account Id: {AccountId}, Position count: {PositionCount}", 
                    request.AccountId, positionDtos.Count());
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error during GetAccountPositionsQuery handling - Account Id: {AccountId}, Exception: {ExceptionMessage}", 
                    request.AccountId, ex.Message);
            }

            return positionDtos;
        }
    }
}
