using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Features.GetAllAccounts
{
    public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, IEnumerable<AccountDto>>
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllAccountsQueryHandler> _logger;

        public GetAllAccountsQueryHandler(IAccountRepository repository, IMapper mapper, ILogger<GetAllAccountsQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }      

        public async Task<IEnumerable<AccountDto>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllAccountsQuery started.");

            var accountDtos = Enumerable.Empty<AccountDto>();

            try
            {
                var accounts = await _repository.GetAllAccountsAsync(cancellationToken);

                accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);

                _logger.LogInformation("Handling GetAllAccountsQuery completed successfully.");                
            }
            catch (Exception)
            {
                _logger.LogInformation("Something wrong happened during GetAllAccountsQuery handling.");                
            }

            return accountDtos;
        }
    }
}
