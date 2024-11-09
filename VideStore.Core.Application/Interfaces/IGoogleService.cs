using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Application.Interfaces
{
    public interface IGoogleService
    {
        Task<Result<AppUserResponse>> GoogleSignInAsync(GoogleRequest credentials);
    }
}
