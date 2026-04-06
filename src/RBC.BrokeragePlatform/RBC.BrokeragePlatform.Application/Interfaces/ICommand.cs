using MediatR;

namespace RBC.BrokeragePlatform.Application.Interfaces
{

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
    
}
