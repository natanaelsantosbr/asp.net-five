using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Natanael.Web.Data;
using Natanael.Web.Domain;
using Natanael.Web.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Natanael.Web.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DataContext context)
        {
            this._userManager = userManager;
            this._jwtSettings = jwtSettings;
            this._tokenValidationParameters = tokenValidationParameters;
            this._context = context;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await this._userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Erros = new[] { "User with this email address already exists" }
                };
            }

            var newUserId = Guid.NewGuid();

            var newUser = new IdentityUser
            {
                Id = newUserId.ToString(),
                Email = email,
                UserName = email                
            };




            var createdUser = await this._userManager.CreateAsync(newUser, password);



            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Erros = createdUser.Errors.Select(a => a.Description)
                };
            }

            await _userManager.AddClaimAsync(newUser, new Claim("role", "Admin"));

            await _userManager.AddToRoleAsync(newUser, "Admin");


            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await this._userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Erros = new[] { "User does not exists" }
                };
            }

            var userHasValidPassword = await this._userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Erros = new[] { "User/password combination is wrong" }
                };
            }

            return await  GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult
                {
                    Erros = new[] { "Invalid Token" }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(a => a.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Erros = new string[] { "This token hasn`t expired yet" }
                };
            }

            var jti = validatedToken.Claims.Single(a => a.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await this._context.RefreshTokens.SingleOrDefaultAsync(a => a.Token == refreshToken);

            if(storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    Erros = new string[] { "This refresh token does not exist" }
                };
            }

            if(DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Erros = new string[] { "This refresh token has expired" }
                };
            }

            if(storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult
                {
                    Erros = new string[] { "This refresh token has been invalidated" }
                };
            }

            if(storedRefreshToken.Used)
            {
                return new AuthenticationResult
                {
                    Erros = new string[] { "This refresh token has been used" }
                };
            }

            if(storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult
                {
                    Erros = new string[] { "This refresh token does not match this JWT" }
                };
            }

            storedRefreshToken.Used = true;
            this._context.RefreshTokens.Update(storedRefreshToken);
            await this._context.SaveChangesAsync();

            var user = await this._userManager.FindByIdAsync(validatedToken.Claims.Single(a => a.Type == "id").Value);

            return await GenerateAuthenticationResultForUserAsync(user);



        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;

                return principal;

            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken)
                && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }



        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(this._jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await this._context.RefreshTokens.AddAsync(refreshToken);
            await this._context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Sucess = true,
                RefreshToken = refreshToken.Token,
                Token = tokenHandler.WriteToken(token)
            };
        }


    }
}
