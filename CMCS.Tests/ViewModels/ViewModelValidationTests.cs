using Xunit;
using System.ComponentModel.DataAnnotations;
using CMCS.ViewModels;

namespace CMCS.Tests.ViewModels
{
    public class ViewModelValidationTests
    {
        [Fact]
        public void LoginViewModel_ValidData_PassesValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@university.edu",
                Password = "password123"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("", "password")] // Empty email
        [InlineData("invalid-email", "password")] // Invalid email format
        [InlineData("test@university.edu", "")] // Empty password
        public void LoginViewModel_InvalidData_FailsValidation(string email, string password)
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = email,
                Password = password
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ClaimItemViewModel_ValidData_PassesValidation()
        {
            // Arrange
            var model = new ClaimItemViewModel
            {
                Date = DateTime.Today,
                HoursWorked = 5,
                HourlyRate = 100,
                ActivityDescription = "Teaching activity"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ClaimItemViewModel_InvalidHoursWorked_FailsValidation()
        {
            // Arrange
            var model = new ClaimItemViewModel
            {
                Date = DateTime.Today,
                HoursWorked = 0, // Invalid - below minimum
                HourlyRate = 100,
                ActivityDescription = "Teaching activity"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
        }
    }
}