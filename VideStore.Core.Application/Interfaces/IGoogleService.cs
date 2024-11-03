using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Requests;
using VideStore.Shared.Responses;

namespace VideStore.Infrastructure.Interfaces
{
    public interface IGoogleService
    {
        Task<Result<AppUserResponse>> GoogleSignInAsync(GoogleRequest credentials);
    }
}
