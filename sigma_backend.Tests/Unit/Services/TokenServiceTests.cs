using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using sigma_backend.Service;
using sigma_backend.Tests.Unit.Helpers;
using Xunit;

namespace sigma_backend.Tests.Unit.Services
{
    public class TokenServiceTests
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TokenService service;

        public TokenServiceTests()
        {
            // Create a symmetric key for signing the token
            var keyBytes = Encoding.UTF8.GetBytes("test_key_at_least_512_bits_so_it_should_be_quite_long_12345678900987654321");
            var key = new SymmetricSecurityKey(keyBytes);
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "test_issuer",
                ValidateAudience = true,
                ValidAudience = "test_audience",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            service = new TokenService(_tokenValidationParameters);

            // Set up environment variables for issuer and audience
            Environment.SetEnvironmentVariable("JWT_ISSUER", "test_issuer");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test_audience");
        }

        [Fact]
        public void GenerateAccessToken_ReturnString()
        {
            // Arrange
            var user = TestUserFactory.CreateTestUser();
            var roles = new List<string> { "User", "Admin" };
            var expiresOn = DateTime.UtcNow.AddMinutes(30);

            // Act
            var token = service.GenerateAccessToken(user, roles, expiresOn);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Check expiration
            Assert.True(jwtToken.ValidTo <= expiresOn.AddSeconds(1)); // Allow slight margin
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);

            // Check issued at (iat)
            Assert.True(jwtToken.ValidFrom <= DateTime.UtcNow);
            Assert.True(jwtToken.IssuedAt <= DateTime.UtcNow);

            // Check issuer and audience
            Assert.Equal("test_issuer", jwtToken.Issuer);
            Assert.Contains(jwtToken.Audiences, a => a == "test_audience");

            // Check user info
            var claims = jwtToken.Claims;
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == user.UserName);
            Assert.Contains(claims, c => c.Type == "role" && c.Value == "User");
            Assert.Contains(claims, c => c.Type == "role" && c.Value == "Admin");
        }
        [Fact]
        public void GenerateRefreshToken_ReturnRefreshTokenAndString()
        {
            // Arrange
            var userId = "test_userId";
            var deviceId = "test_deviceId";
            var expiresOn = DateTime.UtcNow.AddMonths(1);

            // Act
            var (token, tokenBytesString) = service.GenerateRefreshToken(userId, deviceId, expiresOn);

            // Assert
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(tokenBytesString));

            //  Check token object fields
            Assert.Equal(userId, token.UserId);
            Assert.Equal(deviceId, token.DeviceId);
            Assert.Equal(expiresOn, token.ExpiresOn);

            // Check if token hash matches computed hash of rawToken
            string expectedHash = service.ComputeSha256Hash(tokenBytesString);
            Assert.Equal(expectedHash, token.TokenHash);
        }
        [Fact]
        public void GetPrincipalFromToken_ReturnClaimsPrincipal()
        {
            // Arrange
            var user = TestUserFactory.CreateTestUser();
            var roles = new List<string> { "User" };
            var expiresOn = DateTime.UtcNow.AddMinutes(30);

            var token = service.GenerateAccessToken(user, roles, expiresOn);

            // Act
            var principal = service.GetPrincipalFromToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
            Assert.Equal(principal.Identity?.Name, user.UserName);
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
        }
        [Fact]
        public void GetPrincipalFromToken_ReturnNull()
        {
            // Arrange
            var user = TestUserFactory.CreateTestUser();
            var roles = new List<string> { "User" };
            var expiresOn = DateTime.UtcNow.AddMinutes(30);

            var token = service.GenerateAccessToken(user, roles, expiresOn);
            var invalidToken = token.Insert(64, "spoof");

            // Act
            var principal = service.GetPrincipalFromToken(invalidToken);

            // Assert
            Assert.Null(principal);
        }
        [Fact]
        public void VerifyRefreshToken_ReturnTrue()
        {
            // Arrange
            var token = "test-token";
            var hash = service.ComputeSha256Hash(token);

            // Act
            var result = service.VerifyRefreshToken(token, hash);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void VerifyRefreshToken_ReturnFalse()
        {
            // Act
            var result = service.VerifyRefreshToken("test-token", "fake-hash");

            // Assert
            Assert.False(result);
        }
    }
}