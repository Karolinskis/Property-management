using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using O9d.AspNet.FluentValidation;
using RentalManagement.Auth;
using RentalManagement.Contexts;
using RentalManagement.Entities;

namespace RentalManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthenticationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(UserManager<User> userManager, [Validate] RegisterUserDTO registerUserDTO)
        {
            var user = await userManager.FindByNameAsync(registerUserDTO.UserName);
            if (user != null)
                return BadRequest("User already exists.");

            var newUser = new User()
            {
                Email = registerUserDTO.Email,
                UserName = registerUserDTO.UserName

            };

            var result = await userManager.CreateAsync(newUser, registerUserDTO.Password);
            if (!result.Succeeded)
                return UnprocessableEntity(result.Errors);

            foreach (var role in registerUserDTO.Roles)
            {
                if (UserRoles.All.Contains(role))
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
            }

            return Created();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(
            UserManager<User> userManager,
            JwtTokenService jwtTokenService,
            SessionService sessionService,
            LoginUserDTO loginUserDTO)
        {
            var user = await userManager.FindByNameAsync(loginUserDTO.UserName);
            if (user == null)
                return NotFound("User not found.");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDTO.Password);
            if (!isPasswordValid)
                return UnprocessableEntity("Invalid password.");

            var roles = await userManager.GetRolesAsync(user);

            var sessionId = Guid.NewGuid();
            var expiresAt = DateTime.UtcNow.AddDays(3); // TODO: Add time to config
            var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var refreshToken = jwtTokenService.CreateRefreshToken(sessionId, user.Id, expiresAt);

            await sessionService.CreateSessionAsync(sessionId, user.Id, refreshToken, expiresAt);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                // TODO: When in production, set to true
                //Secure = false 
            };

            HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

            return Ok(new SuccessfulLoginDTO { AccessToken = accessToken });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<ActionResult> RefreshToken(
            UserManager<User> userManager,
            SessionService sessionService,
            JwtTokenService jwtTokenService)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return UnprocessableEntity("Refresh token not found.");
            }

            if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
            {
                return UnprocessableEntity("Invalid refresh token.");
            }

            var sessionId = claims.FindFirstValue("SessionId");
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return UnprocessableEntity("Invalid refresh token.");
            }

            var sessionIdAsGuid = Guid.Parse(sessionId);
            if (!await sessionService.IsSessionValidAsync(sessionIdAsGuid, refreshToken))
            {
                return UnprocessableEntity("Invalid refresh token.");
            }

            var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return UnprocessableEntity("User not found.");
            }

            var roles = await userManager.GetRolesAsync(user);

            // TODO: Add to config
            var expiresAt = DateTime.UtcNow.AddDays(3);
            var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var newRefreshToken = jwtTokenService.CreateRefreshToken(sessionIdAsGuid, user.Id, expiresAt);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                //Secure = false TODO: When in production, set to true
            };

            HttpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, cookieOptions);

            await sessionService.ExtendSessionAsync(sessionIdAsGuid, newRefreshToken, expiresAt);

            return Ok(new SuccessfulLoginDTO { AccessToken = accessToken });
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<ActionResult> Logout(
            UserManager<User> userManager,
            SessionService sessionService,
            JwtTokenService jwtTokenService)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return UnprocessableEntity("Refresh token not found.");
            }

            if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
            {
                return UnprocessableEntity("Invalid refresh token.");
            }

            var sessionId = claims.FindFirstValue("SessionId");
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return UnprocessableEntity("Invalid refresh token.");
            }

            await sessionService.InvalidateSessionAsync(Guid.Parse(sessionId));
            HttpContext.Response.Cookies.Delete("RefreshToken");

            return Ok();
        }
    }
}
