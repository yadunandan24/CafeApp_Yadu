using Azure.Identity;
using CafeApi.Models.DataModels;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CafeApi.Common
{
    public class TokenManager
    {
        public static string Secret;
        public static string Issuer;

        public static string GenerateToken(string email, string role)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var myClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("Role",role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                claims:myClaims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials:signingCredentials,
                issuer: Issuer
                );
            return new JwtSecurityTokenHandler().WriteToken(token); 



            //};
            //SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[]
            //    {
            //        new Claim(ClaimTypes.Email,email),
            //        new Claim(ClaimTypes.Role,role)

            //    }),
            //    Expires = DateTime.UtcNow.AddMinutes(120),
            //    SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            //};

            //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    return null;
                }
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidIssuer = Issuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret))
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static TokenClaim ValidateToken(string RawToken)
        {
            string[] array = RawToken.Split(' ');
            var token = array[1];
            ClaimsPrincipal claimsPrincipal = GetPrincipal(token);
            if (claimsPrincipal == null)
                return null;
            ClaimsIdentity claimsIdentity = null;
            try
            {
                claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
            }
            catch (Exception ex)
            {
                return null;
            }

            TokenClaim tokenclaim = new TokenClaim();
            var temp = claimsIdentity.FindFirst(ClaimTypes.Email);
            tokenclaim.Email = temp.Value;
            var temp2 = claimsIdentity.FindFirst("Role");
            tokenclaim.Role = temp2.Value;
            return tokenclaim;

        }
    }
}
