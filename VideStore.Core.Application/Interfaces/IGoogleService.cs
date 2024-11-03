using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Requests.Users;
using VideStore.Shared.Responses.Users;

namespace VideStore.Application.Interfaces
{
    public interface IGoogleService
    {
        Task<Result<AppUserResponse>> GoogleSignInAsync(GoogleRequest credentials);
    }
}
