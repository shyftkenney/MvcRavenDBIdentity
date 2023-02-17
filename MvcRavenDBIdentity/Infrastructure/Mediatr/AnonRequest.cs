using MediatR;

namespace MvcRavenDBIdentity.Infrastructure.Mediatr
{
    public class AnonRequest<TResponse> : IRequest<TResponse>
    {
    }
}
