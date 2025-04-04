using NUnit.Framework;
using Moq;
using FluentValidation;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Repositories;

namespace testing;

public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IValidator<User>> _mockValidator;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockValidator = new Mock<IValidator<User>>();
        _userService = new UserService(_mockUserRepository.Object, _mockValidator.Object);
    }

    [Test]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User { Id = 1, FirstName = "Test User 1", Email = "test1@example.com", PhoneNumber = "1234567890" },
            new User { Id = 2, FirstName = "Test User 2", Email = "test2@example.com", PhoneNumber = "0987654321" }
        };
        _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.That(result, Is.EqualTo(expectedUsers));
        _mockUserRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        int userId = 1;
        var expectedUser = new User { Id = userId, FirstName = "Test User", Email = "test@example.com", PhoneNumber = "1234567890" };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedUser));
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        int userId = 999;
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Null);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task CreateUserAsync_WithValidUser_ShouldCreateAndReturnUser()
    {
        // Arrange
        var newUser = new User { FirstName = "New User", Email = "new@example.com", PhoneNumber = "1234567890" };
        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(validator => validator.ValidateAsync(newUser, default)).ReturnsAsync(validationResult);
        _mockUserRepository.Setup(repo => repo.CreateAsync(newUser)).ReturnsAsync(newUser);

        // Act
        var result = await _userService.CreateUserAsync(newUser);

        // Assert
        Assert.That(result, Is.EqualTo(newUser));
        _mockValidator.Verify(validator => validator.ValidateAsync(newUser, default), Times.Once);
        _mockUserRepository.Verify(repo => repo.CreateAsync(newUser), Times.Once);
    }

    [Test]
    public void CreateUserAsync_WithInvalidUser_ShouldThrowValidationException()
    {
        // Arrange
        var invalidUser = new User { FirstName = "", Email = "invalid-email", PhoneNumber = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Name", "Name is required"),
            new FluentValidation.Results.ValidationFailure("Email", "Invalid email format")
        };
        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);
        _mockValidator.Setup(validator => validator.ValidateAsync(invalidUser, default)).ReturnsAsync(validationResult);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUserAsync(invalidUser));
        _mockValidator.Verify(validator => validator.ValidateAsync(invalidUser, default), Times.Once);
        _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task UpdateUserAsync_WithValidUser_ShouldUpdateUser()
    {
        // Arrange
        int userId = 1;
        var user = new User { Id = userId, FirstName = "Updated User", Email = "updated@example.com", PhoneNumber = "1234567890" };
        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(validator => validator.ValidateAsync(user, default)).ReturnsAsync(validationResult);
        _mockUserRepository.Setup(repo => repo.UpdateAsync(user)).Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(userId, user);

        // Assert
        _mockValidator.Verify(validator => validator.ValidateAsync(user, default), Times.Once);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(user), Times.Once);
    }

    [Test]
    public void UpdateUserAsync_WithInvalidUser_ShouldThrowValidationException()
    {
        // Arrange
        int userId = 1;
        var invalidUser = new User { Id = userId, FirstName = "", Email = "invalid-email", PhoneNumber = "" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Name", "Name is required"),
            new FluentValidation.Results.ValidationFailure("Email", "Invalid email format")
        };
        var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);
        _mockValidator.Setup(validator => validator.ValidateAsync(invalidUser, default)).ReturnsAsync(validationResult);
        
        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUserAsync(userId, invalidUser));
        _mockValidator.Verify(validator => validator.ValidateAsync(invalidUser, default), Times.Once);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task DeleteUserAsync_WithExistingUser_ShouldDeleteUser()
    {
        // Arrange
        int userId = 1;
        _mockUserRepository.Setup(repo => repo.DeleteAsync(userId)).Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(userId);

        // Assert
        _mockUserRepository.Verify(repo => repo.DeleteAsync(userId), Times.Once);
    }
}