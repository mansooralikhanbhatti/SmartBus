using SmartBus.DTOs;

namespace SmartBus.Services
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
