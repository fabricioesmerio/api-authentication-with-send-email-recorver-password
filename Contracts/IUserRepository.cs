using apiLogin.Dto;
using apiLogin.Models;

namespace apiLogin.Contracts
{
    public interface IUserRepository
    {
        Task<Guid> Create(UserCreate input);
        TokenResponse Login(UserLogin input);
        Task<bool> ForgotPassword(string email);
        ValidateCoreResponse ValidateCode(ValidateCode input);
        Task<int> RecoverPassword(RecoverPassword input);
    }
}