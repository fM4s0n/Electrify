using NSubstitute;
using FluentAssertions;
using Electrify.AdminUi.Services;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;
using Microsoft.Extensions.Logging;

namespace Electrify.AdminUi.UnitTests.Services;

public class AdminServiceTests
{
    private readonly AdminService _adminService;
    private readonly IElectrifyApiClient _electrifyApiClient;

    public AdminServiceTests()
    {
        var logger = Substitute.For<ILogger<AdminService>>();
        _electrifyApiClient = Substitute.For<IElectrifyApiClient>();
        _adminService = new AdminService(_electrifyApiClient, logger);
    }

    [Fact]
    public async Task GetCurrentAdmin_Returns_Null_When_No_Admin_Is_Logged_In()
    {
        // Act & Arrange
        var currentAdmin = _adminService.CurrentAdmin;

        // Assert
        currentAdmin.Should().BeNull();
        await _electrifyApiClient.DidNotReceive().AdminLogin(Arg.Any<string>(), Arg.Any<string>());
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

        var expected = ArrangeAdminLogin(email, name, id, token);
        await _adminService.ValidateLogin(email, plainPassword);

        // Act
        var actual = _adminService.CurrentAdmin;

        // Assert
        actual.Should().BeEquivalentTo(expected);
        await _electrifyApiClient.Received(1).AdminLogin(email, plainPassword);
    }

    [Fact]
    public async Task ValidateLogin_Fails_When_Credentials_Are_Invalid()
    {
        // Arrange
        var response = new HttpAdminLoginResponse 
        { 
            Success = false,
            Id = string.Empty,
            Name = string.Empty,
            Email = string.Empty,
            Token = string.Empty
        };

        _electrifyApiClient.AdminLogin(Arg.Any<string>(), Arg.Any<string>())
            .Returns(response);

        // Act
        var result = await _adminService.ValidateLogin("invalid@email.com", "wrongpassword");

        // Assert
        result.Should().BeFalse();
        _adminService.CurrentAdmin.Should().BeNull();
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

        ArrangeAdminLogin(email, name, id, token);
        await _adminService.ValidateLogin(email, plainPassword);

        // Act
        _adminService.LogoutCurrentAdmin();
        var actual = _adminService.CurrentAdmin;

        // Assert
        actual.Should().BeNull();
    }

    [Theory]
    [InlineData(null, "password")]
    [InlineData("email@email.com", null)]
    [InlineData("", "password")]
    [InlineData("email@email.com", "")]
    [InlineData("   ", "password")]
    [InlineData("email@email.com", "   ")]
    public async Task ValidateLogin_Throws_Exception_When_Inputs_Are_Invalid(string? email, string? password)
    {
        // Act
        Func<Task> act = async () => await _adminService.ValidateLogin(email!, password!);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("Value cannot be null or whitespace.*");
    }

    private HttpAdminLoginResponse ArrangeAdminLogin(string email, string name, Guid id, Guid token)
    {
        var response = new HttpAdminLoginResponse
        {
            Success = true,
            Name = name,
            Email = email,
            Token = token.ToString(),
            Id = id.ToString()
        };

        _electrifyApiClient.AdminLogin(Arg.Any<string>(), Arg.Any<string>()).Returns(response);
        return response;
    }
}