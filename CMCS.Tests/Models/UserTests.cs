using Xunit;
using CMCS.Models;

namespace CMCS.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void FullName_ReturnsCorrectFormat()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = user.FullName;

            // Assert
            Assert.Equal("John Doe", result);
        }

        [Fact]
        public void FullName_WithNullFirstName_ReturnsOnlyLastName()
        {
            // Arrange
            var user = new User
            {
                FirstName = null,
                LastName = "Doe"
            };

            // Act
            var result = user.FullName;

            // Assert
            Assert.Equal("Doe", result);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsTrue()
        {
            // Arrange
            var user = new User
            {
                Email = "test@university.edu",
                PasswordHash = "password123"
            };

            // Act
            var result = user.Login("test@university.edu", "password123");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Login_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            var user = new User
            {
                Email = "test@university.edu",
                PasswordHash = "password123"
            };

            // Act
            var result = user.Login("wrong@university.edu", "password123");

            // Assert
            Assert.False(result);
        }
    }
}