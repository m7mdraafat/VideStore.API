using System.Security.Claims;
using VideStore.Application.DTOs;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Requests;

namespace VideStore.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailMessage(EmailResponse email);

    }
}
