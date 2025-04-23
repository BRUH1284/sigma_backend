using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;
using sigma_backend.Models;
using sigma_backend.Tests.Unit.Helpers;
using Xunit;
using MockQueryable;

namespace sigma_backend.Tests.Unit.Services
{
    public class CurrentUserServiceTests
    {
        private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            var queryable = ls.AsQueryable().BuildMock();
            mgr.Setup(x => x.Users).Returns(queryable);

            return mgr;
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var testUser = TestUserFactory.CreateTestUser();
            var userManager = MockUserManager(new List<User> { testUser });
            var service = new CurrentUserService(userManager.Object);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
                new Claim(ClaimTypes.Name, testUser.UserName!)]));

            // Act
            var result = await service.GetCurrentUserAsync(claimsPrincipal);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testUser.UserName!, result?.UserName);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenUsernameIsMissing()
        {
            // Arrange
            var userList = new List<User>(); // No users
            var userManagerMock = MockUserManager(userList);
            var service = new CurrentUserService(userManagerMock.Object);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity()); // No username

            // Act
            var result = await service.GetCurrentUserAsync(claimsPrincipal);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var userList = new List<User>(); // Empty list simulates "not found"
            var userManagerMock = MockUserManager(userList);
            var service = new CurrentUserService(userManagerMock.Object);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([
                new Claim(ClaimTypes.Name, "non-existent-user")]));

            // Act
            var result = await service.GetCurrentUserAsync(claimsPrincipal);

            // Assert
            Assert.Null(result);
        }

    }
}