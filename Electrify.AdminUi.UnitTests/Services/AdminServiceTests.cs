using NSubstitute;
using FluentAssertions;
using Electrify.AdminUi.Services;
using Electrify.Models;
using Electrify.Server.ApiClient.Abstraction;
using Microsoft.AspNetCore.Identity;
using Electrify.Server.ApiClient.Contracts;

namespace Electrify.AdminUi.UnitTests.Services;

public class AdminServiceTests
{
    private readonly AdminService _adminService;
    private readonly IElectrifyApiClient _adminLoginClient;

    public AdminServiceTests()
    {
        _adminLoginClient = Substitute.For<IElectrifyApiClient>();
        _adminService = new AdminService(_adminLoginClient);
    }

    [Fact]
    public void GetCurrentAdmin_Returns_Null_When_No_Admin_Is_Logged_In()
    {
        // Arrange
        // Act
        var currentAdmin = _adminService.CurrentAdmin;

        // Assert
        currentAdmin.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentAdmin_Returns_Admin_When_Admin_Is_Logged_In()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string name = "Test Admin";
        string plainPassword = "password";
        string email = "email@email.com";
        Guid token = Guid.NewGuid();

        var expected = ArrangeAdminLogin(email, name, plainPassword, id, token);

        await _adminService.ValidateLogin(email, plainPassword);

        // Act
        var actual = _adminService.CurrentAdmin;

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options.Excluding(admin => admin.PasswordHash));
    }

    [Fact]
    public async Task LogoutCurrentAdmin_Sets_CurrentAdmin_To_Null()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string name = "Test Admin";
        string plainPassword = "password";
        string email = "email@email.com";
        Guid token = Guid.NewGuid();

        ArrangeAdminLogin(email, name, plainPassword, id, token);

        // Act
        await _adminService.ValidateLogin(email, plainPassword);

        _adminService.LogoutCurrentAdmin();

        var actual = _adminService.CurrentAdmin;

        // Assert
        actual.Should().BeNull();
    }

    private Admin ArrangeAdminLogin(string email, string name, string plainPassword, Guid id, Guid token)
    {
        PasswordHasher<Admin> passwordHasher = new();

        var expected = new Admin
        {
            Id = id,
            Name = name,
            Email = email,
            PasswordHash = string.Empty,
            AccessToken = token,
        };

        string passwordHash = passwordHasher.HashPassword(expected, plainPassword);
        expected.PasswordHash = passwordHash;

        var response = new HttpAdminLoginResponse
        {
            Success = true,
            Name = name,
            Email = email,
            Token = token.ToString(),
            Id = id.ToString()
        };

        _adminLoginClient.AdminLogin(Arg.Any<string>(), Arg.Any<string>()).Returns(response);

        return expected;
    }
}