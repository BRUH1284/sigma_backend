
using sigma_backend.Models;

namespace sigma_backend.Tests.Unit.Helpers
{
    public static class TestUserFactory
    {
        public static User CreateTestUser() => new User
        {
            Email = "johndoe@example.com",
            UserName = "johndoe",
            FirstName = "John",
            LastName = "Doe"
        };
    }
}