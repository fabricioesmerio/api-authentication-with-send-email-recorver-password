using apiLogin.Contracts;
using apiLogin.Dto;
using apiLogin.Models;
using Microsoft.AspNetCore.Mvc;

namespace apiLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepositorio;

        public UserController(IUserRepository userRepository)
        {
            _userRepositorio = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(UserCreate input)
        {
            try
            {
                var id = await _userRepositorio.Create(input);
                if (id == null)
                    return NotFound();

                return Ok(new { Data = id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Login")]
        public ActionResult<User> Login(UserLogin input)
        {
            try
            {
                var token = _userRepositorio.Login(input);
                if (token == null)
                    return NotFound();

                return Ok(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPassword input)
        {
            try
            {
                var result = await _userRepositorio.ForgotPassword(input.email);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("ValidateCode")]
        public ActionResult ValidateCode(ValidateCode input)
        {
            try
            {
                var result = _userRepositorio.ValidateCode(input);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("RecoverPassword")]
        public async Task<ActionResult> RecoverPassword(RecoverPassword input)
        {
            try
            {
                var result = await _userRepositorio.RecoverPassword(input);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}