using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using user.Models;
using Void.Repositories;
using AuthenticationService = Void.Services.AuthenticationService;


namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly UserRepository _userRepository;

        public AuthenticationController(
            AuthenticationService authenticationService,
            UserRepository userRepository)
        {
            _authenticationService = authenticationService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public IActionResult Register(string username, string password, string confirmpassword, string email)
        {


            var user = new User { UserName = username, Password = password, Email = email };
            _userRepository.AddUser(user);

            try
            {
                _authenticationService.Register(username, password, confirmpassword, email);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok($"User {username} registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (_userRepository.UserExists(username) && _userRepository.ValidateUser(username, password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                return Ok("Logged in");
            }
            return Unauthorized("Invalid credentials");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }

        [HttpGet("check")]
        public IActionResult CheckAuth()
        {
            if (User.Identity.IsAuthenticated)
                return Ok($"Hello {User.Identity.Name}");
            return Unauthorized();
        }
    }
}