using NSubstitute;
using FluentAssertions;
using Electrify.AdminUi.Services;
using Electrify.Models.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;

namespace Electrify.AdminUI.UnitTests.Services;

public class AdminServiceTests
{
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        var adminLoginClient = Substitute.For<Server.Protos.AdminLogin.AdminLoginClient>();
        _adminService = new AdminService(adminLoginClient);
    }

    [Fact]
    public void GetCurrentAdmin_Returns_Null_When_No_Admin_Is_Logged_In()
    {
        // Arrange
        // Act
        var currentAdmin = _adminService.GetCurrentAdmin();

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

        var adminLoginClient = Substitute.For<Server.Protos.AdminLogin.AdminLoginClient>();

        // Mock an AsyncUnaryCall
        var adminLoginResponse = new Server.Protos.AdminLoginResponse
        {
            Success = true,
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Token = token.ToString(), 
            Id = id.ToString()
        };

        expected.Id = Guid.Parse(adminLoginResponse.Id);

        var mockAsyncUnaryCall = new AsyncUnaryCall<Server.Protos.AdminLoginResponse>(Task.FromResult(adminLoginResponse), null, null, null, null);

        adminLoginClient.AdminLoginAsync(Arg.Any<Server.Protos.AdminLoginDetailsRequest>()).Returns(mockAsyncUnaryCall);

        var adminService = new AdminService(adminLoginClient);

        await adminService.ValidateLogin("email@email.com", "password");

        // Act
        var actual = adminService.GetCurrentAdmin();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}