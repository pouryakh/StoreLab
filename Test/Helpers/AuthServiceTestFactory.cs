//using Application.DTOs.AuthDto;
//using Application.Interfaces.Repositories;
//using Application.Interfaces.Security;
//using Application.Interfaces.Services;
//using Application.Services;
//using Application.Validators.Auth;
//using Microsoft.Extensions.Logging;
//using Moq;

//namespace Test.Helpers;

//public static class AuthServiceTestFactory
//{
//    public static AuthService Create(
//        Mock<IUserRepository>? userRepository = null,
//        Mock<IUnitOfWork>? unitOfWork = null,
//        Mock<IPasswordHasher>? passwordHasher = null)
//    {
//        userRepository ??= new Mock<IUserRepository>();
//        unitOfWork ??= new Mock<IUnitOfWork>();
//        passwordHasher ??= new Mock<IPasswordHasher>();

//        var registerValidator = new RegisterValidator();

//        var logger = new Mock<ILogger<AuthService>>();

//        return new AuthService(
//            userRepository.Object,
//            unitOfWork.Object,
//            passwordHasher.Object,
//            registerValidator,
//            logger.Object);
//    }
//}