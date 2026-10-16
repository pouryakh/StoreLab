//using Application.DTOs.AuthDto;
//using Application.Interfaces.Repositories;
//using Application.Interfaces.Security;
//using Application.Results;
//using Application.Services;
//using Application.Validators.Auth;
//using Castle.Core.Logging;
//using Domain.Entities;
//using Domain.Enums;
//using FluentValidation;
//using FluentValidation.Results;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System.ComponentModel.DataAnnotations;
//using Test.Helpers;

//namespace Marketplace.Tests.Services;

//public class AuthServiceTests
//{
//    [Fact]
//    public async Task RegisterAsync_Should_Register_User_Successfully()
//    {
//        //Arrange

//        var userRepository = new Mock<IUserRepository>();
//        var unitOfWork = new Mock<IUnitOfWork>();
//        var passwordHasher = new Mock<IPasswordHasher>();


//        var request = new RegisterRequest
//        {
//            FirstName = "Pourya",
//            LastName = "Test",
//            Email = "pourya@test.com",
//            PhoneNumber = "09123456789",
//            Password = "Password123!",
//            ConfirmPassword = "Password123!"
//        };

//        userRepository.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
//        userRepository.Setup(x => x.GetByPhoneNumberAsync(request.PhoneNumber)).ReturnsAsync((User?)null);
//        passwordHasher.Setup(x => x.Hash(request.Password)).Returns("Hash-Password");

//        var authService = AuthServiceTestFactory.Create(
//        userRepository,
//        unitOfWork,
//        passwordHasher);

//        //Act

//        var result = await authService.RegisterAsync(request);

//        //Assert

//        Assert.True(result.IsSuccess);
//        Assert.NotNull(result.Data);

//        userRepository.Verify(
//            x => x.AddAsync(It.Is<User>(u =>
//                u.Email == request.Email &&
//                u.FirstName == request.FirstName &&
//                u.LastName == request.LastName &&
//                u.PasswordHash == "Hash-Password" &&
//            u.Role == UserRole.Buyer)),
//            Times.Once);
//        passwordHasher.Verify(x => x.Hash(request.Password), Times.Once);
//        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

//    }

//    [Fact]
//    public async Task RegisterAsync_Should_Fail_When_PhoneNumber_Already_Exists() 
//    {
//        //Arrange

//        var userRepository = new Mock<IUserRepository>();
//        var unitOfWork = new Mock<IUnitOfWork>();
//        var passwordHasher = new Mock<IPasswordHasher>();

        

//        var request = new RegisterRequest
//        {
//            FirstName = "Pourya",
//            LastName = "Test",
//            Email = "pourya@test.com",
//            PhoneNumber = "09123456789",
//            Password = "Password123!",
//            ConfirmPassword = "Password123!"
//        };

//        var existingUser = new User(
//            "Ali",
//            "Ahmadi",
//            "Ali@test.com",
//            "Existing-Hash",
//            request.PhoneNumber,
//            UserRole.Buyer);

//        userRepository.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
//        userRepository.Setup(x => x.GetByPhoneNumberAsync(request.PhoneNumber)).ReturnsAsync(existingUser);

//        var authService = AuthServiceTestFactory.Create(
//        userRepository,
//        unitOfWork,
//        passwordHasher);

//        //Act

//        var result = await authService.RegisterAsync(request);

//        //Assert

//        Assert.False(result.IsSuccess);
//        Assert.Null(result.Data);
//        Assert.Equal("PhoneNumber already exists.", result.Error);

//        userRepository.Verify(
//            x => x.AddAsync(It.IsAny<User>()),
//            Times.Never);

//        unitOfWork.Verify(
//            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
//            Times.Never);

//        passwordHasher.Verify(
//            x => x.Hash(It.IsAny<string>()),
//            Times.Never);
//    }

//    [Fact]
//    public async Task RegisterAsync_Should_Fail_When_Email_Already_Exists() 
//    {
//        //Arrange

//        var userRepository = new Mock<IUserRepository>();
//        var unitOfWork = new Mock<IUnitOfWork>();
//        var passwordHasher = new Mock<IPasswordHasher>();

//        var validator = new RegisterValidator();

        

//        var request = new RegisterRequest
//        {
//            FirstName = "Pourya",
//            LastName = "Test",
//            Email = "pourya@test.com",
//            PhoneNumber = "09123456789",
//            Password = "Password123!",
//            ConfirmPassword = "Password123!"
//        };

//        var existingUser = new User(
//            "Ali",
//            "Ahmadi",
//            request.Email,
//            "Existing-Hash",
//            request.PhoneNumber,
//            UserRole.Buyer);

//        userRepository.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);

//        var authService = AuthServiceTestFactory.Create(
//        userRepository,
//        unitOfWork,
//        passwordHasher);

//        //Act

//        var result = await authService.RegisterAsync(request);

//        //Assert

//        Assert.False(result.IsSuccess);
//        Assert.Null(result.Data);
//        Assert.Equal("Email already exists.", result.Error);

//        userRepository.Verify(
//            x => x.AddAsync(It.IsAny<User>()),
//            Times.Never);

//        unitOfWork.Verify(
//            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
//            Times.Never);

//        passwordHasher.Verify(
//            x => x.Hash(It.IsAny<string>()),
//            Times.Never);




//    }

//    [Fact]
//    public async Task RegisterAsync_Should_Fail_When_Request_Is_Invalid()
//    {
//        // Arrange

//        var userRepository = new Mock<IUserRepository>();
//        var unitOfWork = new Mock<IUnitOfWork>();
//        var passwordHasher = new Mock<IPasswordHasher>();

//        var validator = new RegisterValidator();

//        var request = new RegisterRequest
//        {
//            FirstName = "Pourya",
//            LastName = "Test",
//            Email = "invalid-email",
//            PhoneNumber = "09123456789",
//            Password = "Password123!",
//            ConfirmPassword = "Password123!"
//        };

//        var authService = AuthServiceTestFactory.Create(
//        userRepository,
//        unitOfWork,
//        passwordHasher);

//        // Act

//        var result = await authService.RegisterAsync(request);

//        // Assert

//        Assert.False(result.IsSuccess);
//        Assert.Null(result.Data);
//        Assert.NotNull(result.Error);

//        userRepository.Verify(
//            x => x.GetByEmailAsync(It.IsAny<string>()),
//            Times.Never);

//        userRepository.Verify(
//            x => x.GetByPhoneNumberAsync(It.IsAny<string>()),
//            Times.Never);

//        passwordHasher.Verify(
//            x => x.Hash(It.IsAny<string>()),
//            Times.Never);

//        userRepository.Verify(
//            x => x.AddAsync(It.IsAny<User>()),
//            Times.Never);

//        unitOfWork.Verify(
//            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
//            Times.Never);
//    }
//}