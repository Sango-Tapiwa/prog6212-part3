using Xunit;
using CMCS.Models;

namespace CMCS.Tests.Models
{
    public class ClaimItemTests
    {
        [Fact]
        public void CalculateLineTotal_ReturnsCorrectValue()
        {
            // Arrange
            var claimItem = new ClaimItem
            {
                HoursWorked = 5,
                HourlyRate = 100
            };

            // Act
            var result = claimItem.CalculateLineTotal();

            // Assert
            Assert.Equal(500, result);
        }

        [Fact]
        public void LineTotal_Property_ReturnsCalculatedValue()
        {
            // Arrange
            var claimItem = new ClaimItem
            {
                HoursWorked = 2,
                HourlyRate = 75
            };

            // Act & Assert
            Assert.Equal(150, claimItem.LineTotal);
        }
    }
}