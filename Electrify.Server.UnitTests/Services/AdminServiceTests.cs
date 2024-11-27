using Electrify.Models;
using Electrify.Server.Database;
using Electrify.Server.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Electrify.Server.UnitTests.Services;

public class AdminServiceTests
{
    private readonly AdminService _adminService;
    private readonly PasswordHasher<Admin> _passwordHasher;
    private readonly ElectrifyDbContext _database;

    public AdminServiceTests()
    {
        var builder = new DbContextOptionsBuilder<ElectrifyDbContext>();
        builder.UseInMemoryDatabase("UnitTestDb");
        _passwordHasher = new PasswordHasher<Admin>();
        _database = new ElectrifyDbContext(builder.Options);
        _adminService = new AdminService(_database, _passwordHasher);
    }

    [Fact]
    public async Task CreateAdmin_Should_Create_Valid_Admin()
    {
        // Arrange
        const string name = "John Doe";
        const string email = "test@test.com";
        const string plainTextPassword = "password";

        // Act
        await _adminService.CreateAdmin(name, email, plainTextPassword);

        var admin = _database.Admins.FirstOrDefault(a => a.Email == email);

        // Assert
        admin.Should().NotBeNull();

        using (new AssertionScope())
        {           
            admin!.Name.Should().Be(name);
            admin.Email.Should().Be(email);
            admin.PasswordHash.Should().NotBeNullOrEmpty();
            admin.PasswordHash.Should().NotBe(plainTextPassword);
        }
    }

    [Fact]
    public async Task VerifyPassword_Should_Return_True()
    {
        // Arrange
        const string name = "Lewis Hamilton";
        const string email = "test1@mercedes.com";
        const string plainTextPassword = "password1";
        await _adminService.CreateAdmin(name, email, plainTextPassword);

        // Act
        var admin = _database.Admins.FirstOrDefault(a => a.Email == email);
        bool result = _adminService.VerifyPassword(admin!, plainTextPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyPassword_Should_Return_False()
    {
        // Arrange
        const string name = "Charles Leclerc";
        const string email = "test@ferrari.com";
        const string plainTextPassword = "password2";
        await _adminService.CreateAdmin(name, email, plainTextPassword);

        // Act
        var admin = _database.Admins.FirstOrDefault(a => a.Email == email);
        bool result = _adminService.VerifyPassword(admin!, "WrongPassword");

        // Assert
        result.Should().BeFalse();
    }
}
