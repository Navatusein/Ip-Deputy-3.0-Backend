using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace IpDeputyApi.Controllers.Frontend
{
    [Tags("Frontend Authentication Controller")]
    [Route("api/frontend/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string CookieKey = "RefreshToken";

        private static Serilog.ILogger Logger => Serilog.Log.ForContext<AuthenticationController>();
        private readonly IpDeputyDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthenticationController(IpDeputyDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            Logger.Debug("Start Login(loginDto: {@loginDto})", loginDto);
            var webAdminAuth = await _context.WebAuthentications
                .Where(x => x.Login == loginDto.Login.ToLower())
                .FirstOrDefaultAsync();

            if (webAdminAuth == null)
            {
                Logger.Debug("Error Login: webAdminAuth Is Null");
                return BadRequest("Invalid user login or password");
            }
            
            if (!webAdminAuth.VerifyPasswordHash(loginDto.Password))
            {
                Logger.Debug("Error Login: Invalid password");
                return BadRequest("Invalid user login or password");
            }
            
            var userDto = _jwtHelper.GetFrontendUserDto(webAdminAuth.Student, out var refreshToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc).AddDays(30),
                Secure = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append(CookieKey, refreshToken, cookieOptions);

            Logger.Debug("Result Login(userDto: {@userDto} refreshToken: {@refreshToken})", userDto, refreshToken);
            return Ok(userDto);
        }
        
        [AllowAnonymous]
        [Route("login-bot")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> LoginBot(int telegramId)
        {
            Logger.Debug("Start LoginBot(telegramId: {@telegramId})", telegramId);

            var telegram = await _context.Telegrams.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
            
            if (telegram == null)
            {
                Logger.Debug("Error LoginBot: telegram Is Null");
                return BadRequest("Not authorized student");
            }

            var userDto = _jwtHelper.GetFrontendUserDto(telegram.Student, out var refreshToken);

            Logger.Debug("Result LoginBot(userDto: {@userDto})", userDto);
            return Ok(userDto);
        }

        [AllowAnonymous]
        [Route("refresh")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> Refresh(UserDto userDto)
        {
            var refreshToken = Request.Cookies[CookieKey];
            Logger.Debug("Start Refresh(userDto: {@userDto} refreshToken: {refreshToken})", userDto, refreshToken);

            if (refreshToken == null)
            {
                Logger.Debug("Error Refresh: refreshToken Is Null");
                return BadRequest("Invalid refresh token");
            }
            
            if (!_jwtHelper.ValidateRefreshJwt(refreshToken, userDto.StudentId))
            {
                Logger.Debug("Error Refresh: Invalid refreshToken");
                return BadRequest("Invalid refresh token");
            }

            Student? student = await _context.Students.FirstOrDefaultAsync(x => x.Id == userDto.StudentId);

            if (student == null)
            {
                Logger.Debug("Error Refresh: student Is Null");
                return BadRequest("Invalid user data");
            }

            userDto = _jwtHelper.GetFrontendUserDto(student, out refreshToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc).AddDays(30),
                Secure = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

            Logger.Debug("Result Refresh(userDto: {@userDto} refreshToken: {@refreshToken})", userDto, refreshToken);
            return Ok(userDto);
        }

        [AllowAnonymous]
        [Route("password")]
        [HttpGet]
        public ActionResult<Dictionary<string, string>> GeneratePassword(string password)
        {
            Logger.Debug("Start GeneratePassword(password: {@password})", password);
            Dictionary<string, string> passwordData = new();

            using var hmac = new HMACSHA512();
            passwordData["Salt"] = Convert.ToBase64String(hmac.Key);
            passwordData["Hash"] = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

            Logger.Debug("Result GeneratePassword(passwordData: {@passwordData})", passwordData);
            return passwordData;
        }
    }
}
