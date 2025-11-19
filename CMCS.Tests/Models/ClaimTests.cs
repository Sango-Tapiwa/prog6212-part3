using Xunit;
using CMCS.Models;

namespace CMCS.Tests.Models
{
    public class ClaimTests
    {
        [Fact]
        public void CalculateTotal_WithClaimItems_ReturnsCorrectSum()
        {
            // Arrange
            var claim = new Claim();
            claim.ClaimItems.Add(new ClaimItem
            {
                HoursWorked = 5,
                HourlyRate = 100
            });
            claim.ClaimItems.Add(new ClaimItem
            {
                HoursWorked = 3,
                HourlyRate = 150
            });

            // Act
            var result = claim.CalculateTotal();

            // Assert
            Assert.Equal(950, result); // (5*100) + (3*150) = 500 + 450 = 950
        }

        [Fact]
        public void CalculateTotal_WithNoClaimItems_ReturnsZero()
        {
            // Arrange
            var claim = new Claim();

            // Act
            var result = claim.CalculateTotal();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void UpdateStatus_ToApproved_SetsApprovedDate()
        {
            // Arrange
            var claim = new Claim();

            // Act
            claim.UpdateStatus(ClaimStatus.Approved, "Test approval");

            // Assert
            Assert.Equal(ClaimStatus.Approved, claim.Status);
            Assert.NotNull(claim.ApprovedDate);
            Assert.Equal("Test approval", claim.ApprovalComments);
        }

        [Fact]
        public void UpdateStatus_ToRejected_SetsApprovedDate()
        {
            // Arrange
            var claim = new Claim();

            // Act
            claim.UpdateStatus(ClaimStatus.Rejected, "Test rejection");

            // Assert
            Assert.Equal(ClaimStatus.Rejected, claim.Status);
            Assert.NotNull(claim.ApprovedDate);
            Assert.Equal("Test rejection", claim.ApprovalComments);
        }

        [Fact]
        public void UpdateStatus_ToUnderReview_DoesNotSetApprovedDate()
        {
            // Arrange
            var claim = new Claim();

            // Act
            claim.UpdateStatus(ClaimStatus.UnderReview);

            // Assert
            Assert.Equal(ClaimStatus.UnderReview, claim.Status);
            Assert.Null(claim.ApprovedDate);
        }
    }
}