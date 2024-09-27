using Electrify.DlmsServer.Services;
using Electrify.Models.Models;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Electrify.UnitTests.DlmsServer.Services;

public class AdminServiceTests
{
    private readonly AdminService _adminService;
    private readonly Admin _admin;

    private readonly string _name = "John Doe";
    private readonly string _email = "test@test.com";
    private readonly string _plainTextPassword = "password";

    public AdminServiceTests()
    {
        _adminService = new AdminService();
        _admin = _adminService.CreateAdmin(_name, _email, _plainTextPassword);
    }

    [Fact]
    public void CreateAdmin_Should_Create_Valid_Admin()
    {
        // Act
        Admin admin = _adminService.CreateAdmin(_name, _email, _plainTextPassword);

        // Assert
        using (new AssertionScope())
        {
            admin.Should().NotBeNull();
            admin.Name.Should().Be(_name);
            admin.Email.Should().Be(_email);
            admin.PasswordHash.Should().NotBeNullOrEmpty();
            admin.PasswordHash.Should().NotBe(_plainTextPassword);
        } 
    }

    [Fact]
    public void VerifyPassword_Should_Return_True()
    {
        // Act
        bool result = _adminService.VerifyPassword(_admin, _plainTextPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False()
    {
        // Act
        bool result = _adminService.VerifyPassword(_admin, "WrongPassword");

        // Assert
        result.Should().BeFalse();
    }
}

