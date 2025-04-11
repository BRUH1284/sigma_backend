using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using sigma_backend.Interfaces;
using sigma_backend.Models;

namespace sigma_backend.Service
{
    public class TokenService : ITokenService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenService(TokenValidationParameters tokenValidationParameters)
        {
            _tokenValidationParameters = tokenValidationParameters;
        }
        public string GenerateAccessToken(User user, IList<string> roles, DateTime expiresOn)
        {
            // Add user-specific claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Signing credentials using the symmetric key and HMAC SHA512
            var credentials = new SigningCredentials(_tokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha512);

            // Describe the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresOn,
                SigningCredentials = credentials,
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            };

            // Create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Return the serialized token string
            return tokenString;
        }

        public (RefreshToken, string) GenerateRefreshToken(string userId, string deviceId, DateTime expiresOn)
        {
            // Generate random bytes
            var randomBytes = new byte[64];
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            // Convert the random bytes to a base64 string
            var bytesString = Convert.ToBase64String(randomBytes);

            // Create token hash
            var tokenHash = ComputeSha256Hash(bytesString);

            // Return new refresh token
            return (new RefreshToken
            {
                UserId = userId,
                DeviceId = deviceId,
                TokenHash = tokenHash,
                ExpiresOn = expiresOn
            },
            bytesString);
        }

        public bool VerifyRefreshToken(string refreshToken, string tokenHash)
        {
            return tokenHash == ComputeSha256Hash(refreshToken);
        }
        private string ComputeSha256Hash(string rawToken)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(rawToken);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken)
        {
            // Define token validation parameters
            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate the token and extract the claims principal and the security token
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            // Cast the security token to a JwtSecurityToken for further validation.
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            // Ensure the token is a valid JWT and uses the HmacSha512 signing algorithm
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
    }
}