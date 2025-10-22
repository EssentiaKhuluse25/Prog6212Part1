using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog6212Part1.Controllers;
using Prog6212Part1.Models;
using Prog6212Part1.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Prog6212Part1.Tests
{
    public class ClaimsControllerTests
    {
        private ClaimsController _controller;
        private ApplicationDbContext _context;

        public ClaimsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ClaimsTestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(m => m.WebRootPath).Returns("wwwroot");

            _controller = new ClaimsController(_context, mockEnv.Object);
        }

        [Fact]
        public void GetClaimsForm_ReturnsViewResult()
        {
            // Act
            var result = _controller.Claims();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PostClaims_ValidClaim_SavesToDb()
        {
            // Arrange
            var claim = new Claims
            {
                LecturerID = "123",
                FirstName = "John",
                LastName = "Doe",
                ClaimsPeriodsStart = new System.DateTime(2023, 1, 1),
                ClaimsPeriodsEnd = new System.DateTime(2023, 1, 5),
                HoursWorked = 10,
                RateHour = 100,
                DescriptionOfWork = "Teaching"
            };

            // Act
            var result = await _controller.Claims(claim);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ClaimSubmitted", redirect.ActionName);

            var savedClaim = _context.Claims.FirstOrDefault(c => c.LecturerID == "123");
            Assert.NotNull(savedClaim);
            Assert.Equal(1000, savedClaim.TotalHours);
        }

      
    }
}
