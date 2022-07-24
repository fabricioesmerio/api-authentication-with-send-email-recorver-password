using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using apiLogin.Common.Services;
using apiLogin.Contracts;
using apiLogin.Data;
using apiLogin.Dto;
using apiLogin.Helpers;
using apiLogin.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace apiLogin.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SystemContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMailService _mailService;

        public UserRepository(
            SystemContext context,
            IOptions<AppSettings> appSettings,
            IMailService mailService
        )
        {
            _context = context;
            _appSettings = appSettings.Value;
            _mailService = mailService;
        }

        public async Task<Guid> Create(UserCreate input)
        {
            try
            {
                var user = new User();
                user.Id = Guid.NewGuid();
                user.Email = input.Email;
                user.Name = input.Name;
                user.Password = AESEncrytDescry.HashGenerator(input.Password);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TokenResponse Login(UserLogin input)
        {
            var user = UserByEmail(input.Email);

            if (user == null)
                throw new Exception("Usuário e/ou senha incorretos.");

            var compare = AESEncrytDescry.CompareHash(input.Password, user.Password);

            if (!compare)
                throw new Exception("Usuário e/ou senha incorretos.");

            TokenResponse token = new TokenResponse();

            token.Token = GenerateJwtToken(user);

            return token;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var user = UserByEmail(email);
            if (user != null)
            {

                Random rdn = new Random();
                DateTime currentTime = DateTime.Now;

                var solicitacao = new Solicitacao();
                solicitacao.Id = Guid.NewGuid();
                solicitacao.UsuarioId = user.Id;
                solicitacao.Codigo = rdn.Next(100000, 1000000);
                solicitacao.Vencimento = currentTime.AddMinutes(5).ToUniversalTime();

                _context.Solicitacoes.Add(solicitacao);
                _context.SaveChanges();
                await SendEmailForgotPassword(user, solicitacao);
            }
            return true;
        }

        public ValidateCoreResponse ValidateCode(ValidateCode input)
        {
            var response = new ValidateCoreResponse();
            response.Success = false;
            response.Message = "O código informado é inválido";

            var solicitacao = _context.Solicitacoes.FirstOrDefault(s => s.Codigo == input.Codigo);
            if (solicitacao != null && solicitacao.Vencimento > DateTime.Now)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == input.Email);
                if (user != null)
                {
                    if (user.Id == solicitacao.UsuarioId)
                    {
                        response.Message = string.Empty;
                        response.Success = true;
                        _context.Solicitacoes.Remove(solicitacao);
                        _context.SaveChanges();
                    }
                }
            }
                

            return response;
        }

        public async Task<int> RecoverPassword(RecoverPassword input)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == input.Email);
            if (user != null)
            {
                user.Password = AESEncrytDescry.HashGenerator(input.NovaSenha);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return 1;
            }
            return 0;
        }

        private async Task SendEmailForgotPassword(User user, Solicitacao sol)
        {
            await _mailService.SendEmailAsync(user, sol);
        }

        private User UserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}