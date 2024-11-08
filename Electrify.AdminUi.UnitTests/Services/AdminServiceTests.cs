using NSubstitute;
using FluentAssertions;
using Electrify.AdminUi.Services;
using Electrify.Models.Models;
using Electrify.Server.ApiClient.Abstraction;
using Microsoft.AspNetCore.Identity;
using Electrify.Server.ApiClient.Contracts;

namespace Electrify.AdminUI.UnitTests.Services;

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
        PasswordHasher<Admin> passwordHasher = new();

        Guid id = Guid.NewGuid();
        string name = "Test Admin";
        string plainPassword = "password";
        string email = "email@email.com";
        string passwordHash;
        Guid token = Guid.NewGuid();

        var expected = new Admin
        {
            Id = id,
            Name = name,
            Email = email,
            PasswordHash = string.Empty,
            AccessToken = token,
        };

        passwordHash = passwordHasher.HashPassword(expected, plainPassword);
        expected.PasswordHash = passwordHash;

        var response = new HttpAdminLoginResponse
        {
            Success = true,
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Token = token.ToString(),
            Id = id.ToString()
        };

        _adminLoginClient.AdminLogin(Arg.Any<string>(), Arg.Any<string>()).Returns(response);

        await _adminService.ValidateLogin(email, plainPassword);

        // Act
        var actual = _adminService.CurrentAdmin;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}