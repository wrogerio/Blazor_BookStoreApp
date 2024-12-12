using AutoMapper;
using BookStoreApp.Api.Data;
using BookStoreApp.Api.Models.User;
using BookStoreApp.Api.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto userDto)
    {
        try
        {
            var user = _mapper.Map<ApiUser>(userDto);
            user.UserName = user.Email;
            user.NormalizedUserName = user.Email.ToUpper();
            var userCreated = await _userManager.CreateAsync(user, userDto.Password);

            if (!userCreated.Succeeded)
            {
                foreach (var error in userCreated.Errors)
                {
                    _logger.LogError("Error creating user: {0}", error.Description);
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            // If role exists, add user to role
            var rolesList = await _userManager.GetRolesAsync(user);
            if (rolesList.Contains(userDto.Role))
            {
                await _userManager.AddToRoleAsync(user, userDto.Role);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Problem($"Something went wrong at {nameof(Register)}: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginUserDto loginUserDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user is null)
            {
                _logger.LogError("User not found");
                return Unauthorized();
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!passwordValid)
            {
                _logger.LogError("Invalid password");
                return Unauthorized();
            }

            var tokenString = await GenerateToken(user);
            var response = new AuthResponse
            {
                UserId = user.Id,
                Token = tokenString,
                Email = user.Email!
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            return Problem($"Something went wrong at {nameof(Login)}: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private async Task<string> GenerateToken(ApiUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(CustomClaimTypes.UserName, user.UserName!),
            new Claim(CustomClaimTypes.JTI, Guid.NewGuid().ToString()),
            new Claim(CustomClaimTypes.Uid, user.Id),
            new Claim(CustomClaimTypes.Email, user.Email!),
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
            claims.Add(new Claim(CustomClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(Convert.ToInt16(_configuration["JwtSettings:Duration"]!)),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }
}
