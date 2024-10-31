using Electrify.Server.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Electrify.Server.Database;
using Electrify.Server.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Electrify.UnitTests.Server.Services;

public class AdminServiceTests
{
    private readonly IAdminService _adminService;
    private readonly ElectrifyDbContext _database;

    public AdminServiceTests()
    {
        var builder = new DbContextOptionsBuilder<ElectrifyDbContext>();
        builder.UseInMemoryDatabase("UnitTestDb");
        _database = new ElectrifyDbContext(builder.Options);
        _adminService = new AdminService(_database);
    }

    [Fact]
    public void CreateAdmin_Should_Create_Valid_Admin()
    {
        // Arrange
        string name = "John Doe";
        string email = "test@test.com";
        string plainTextPassword = "password";

        // Act
        _adminService.CreateAdmin(name, email, plainTextPassword);

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
    public void VerifyPassword_Should_Return_True()
    {
        // Arrange
        string name = "Lewis Hamilton";
        string email = "test1@mercedes.com";
        string plainTextPassword = "password1";
        _adminService.CreateAdmin(name, email, plainTextPassword);

        // Act
        var admin = _database.Admins.FirstOrDefault(a => a.Email == email);
        bool result = _adminService.VerifyPassword(admin!, plainTextPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False()
    {
        // Arrange
        string name = "Charles Leclerc";
        string email = "test@ferrari.com";
        string plainTextPassword = "password2";
        _adminService.CreateAdmin(name, email, plainTextPassword);

        // Act
        var admin = _database.Admins.FirstOrDefault(a => a.Email == email);
        bool result = _adminService.VerifyPassword(admin!, "WrongPassword");

        // Assert
        result.Should().BeFalse();
    }
}

