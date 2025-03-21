using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XAct;

namespace CibusServer.Services
{
    public class JwtService
    {
        public static string secret { private get; set; } = "";
        public static string expDate { private get; set; } = "";

        private static readonly ConcurrentDictionary<string, int> _messageidToken = new ConcurrentDictionary<string, int>();

        public JwtService()
        {
        }

        public virtual string GenerateSecurityToken_ByName(string userName, object userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, $"{userId}")
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(expDate)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        public bool IsTokenExist(string token)
        {
            return _messageidToken.ContainsKey(token);
        }

        public void SaveToken(int id, string token)
        {
            _messageidToken[token] = id;
        }

        public virtual void RemoveToken(string token)
        {
            if (_messageidToken.ContainsKey(token))
            {
                _messageidToken.Remove(token, out var value);
            }            
        }

        public virtual int? GetUserId(string token)
        {
           if (_messageidToken.TryGetValue(token, out var userId))
            {
                return userId;
            }
           else
            {
                return null;
            }
        }
    }

   
}
