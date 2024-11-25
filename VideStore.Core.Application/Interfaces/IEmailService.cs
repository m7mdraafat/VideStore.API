
using VideStore.Application.DTOs;


namespace VideStore.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailMessage(EmailResponse email);

    }
}
