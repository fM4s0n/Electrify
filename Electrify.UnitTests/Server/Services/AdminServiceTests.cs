using Electrify.DlmsServer.Services;
using Electrify.Models.Models;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Electrify.UnitTests.DlmsServer.Services;

public class AdminServiceTests
{
    [Fact]
    public void CreateAdmin_Should_Create_Valid_Admin()
    {
        // Arrange
        var adminService = new AdminService();
        string name = "John Doe";
        string email = "test@test.com";
        string plainTextPassword = "password";

        // Act
        Admin admin = adminService.CreateAdmin(name, email, plainTextPassword);

        // Assert
        using (new AssertionScope())
        {
            admin.Should().NotBeNull();
            admin.Name.Should().Be(name);
            admin.Email.Should().Be(email);
            admin.PasswordHash.Should().NotBeNullOrEmpty();
            admin.PasswordHash.Should().NotBe(plainTextPassword);
        } 
    }

    [Fact]
    public void VerifyPassword_Should_Return_True()
    {
        // Arrange
        var adminService = new AdminService();
        string name = "Lewis Hamilton";
        string email = "test1@mercedes.com";
        string plainTextPassword = "password1";
        var admin = adminService.CreateAdmin(name, email, plainTextPassword);

        // Act
        bool result = adminService.VerifyPassword(admin, plainTextPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False()
    {
        // Arrange
        var adminService = new AdminService();
        string name = "Charles Leclerc";
        string email = "test@ferrari.com";
        string plainTextPassword = "password2";
        var admin = adminService.CreateAdmin(name, email, plainTextPassword);

        // Act
        bool result = adminService.VerifyPassword(admin, "WrongPassword");

        // Assert
        result.Should().BeFalse();
    }
}

