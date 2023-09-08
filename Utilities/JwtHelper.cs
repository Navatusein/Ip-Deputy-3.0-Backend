using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IpDeputyApi.Utilities
{
    public class JwtHelper
    {
        private static Serilog.ILogger Logger => Serilog.Log.ForContext<JwtHelper>();
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public UserDto GetFrontendUserDto(Student student, out string refreshToken, bool refresh = true)
        {
            UserDto userDto = new()
            {
                StudentId = student.Id,
                UserName = student.Name,
                JwtToken = GetAuthorizeJwt(student.Id, refresh),
            };

            refreshToken = "";
            
            if (refresh) 
                refreshToken = GetRefreshJwt(student.Id);
            
            return userDto;
        }

        public bool ValidateRefreshJwt(string token, int studentId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["FrontendRefreshJWT:Issuer"],
                ValidAudience = _config["FrontendRefreshJWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["FrontendRefreshJWT:Key"]!))
            };

            if (!tokenHandler.CanReadToken(token)) 
                return false;
            
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (principal.HasClaim(c => c.Type == "id"))
                {
                    var id = Convert.ToInt32(principal.Claims.First(c => c.Type == "id").Value);
                    return studentId == id;
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Jwt error: @1", exception);
            }

            return false;
        }
        
        private string GetAuthorizeJwt(int studentId, bool refresh = true)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["FrontendAuthorizeJWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", studentId.ToString()),
                new Claim("refresh", refresh.ToString())
            };

            var token = new JwtSecurityToken(_config["FrontendAuthorizeJWT:Issuer"],
                _config["FrontendAuthorizeJWT:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private string GetRefreshJwt(int studentId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["FrontendRefreshJWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", studentId.ToString())
            };

            var token = new JwtSecurityToken(
                _config["FrontendRefreshJWT:Issuer"],
                _config["FrontendRefreshJWT:Audience"],
                claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
