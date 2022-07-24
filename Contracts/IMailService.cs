using apiLogin.Models;

namespace apiLogin.Contracts
{
    public interface IMailService
    {
        Task SendEmailAsync(User user, Solicitacao solicitacao);
    }
}