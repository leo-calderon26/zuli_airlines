using Microsoft.Extensions.Configuration;
using Mapster;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Mappings;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class UserRegistrationServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IConfiguration> _configurationMock;

        private UserRegistrationService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _configurationMock = new Mock<IConfiguration>();

            var config = TypeAdapterConfig.GlobalSettings;
            new UserRegistrationMappingConfig().Register(config);
            new UserUpdateMappingConfig().Register(config);

            _service = new UserRegistrationService(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                new RegisterUserValidator(),
                new ActivateAccountValidator(),
                _configurationMock.Object);
        }

        [Test]
        public async Task UpdateUser_ValidRequest_UpdatesUserAndReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var existingUser = new AppUser
            {
                UserId = userId,
                FirstName = "Old",
                FirstLastName = "Value",
                SecondLastName = "Here",
                BusinessEmail = "old@ucr.ac.cr",
                Email = "old@ucr.ac.cr",
                UserRole = "Operator"
            };

            var request = new RegisterUserRequestDTO
            {
                NationalId = "123456789",
                BusinessEmail = "  NEW.USER@ucr.ac.cr  ",
                FirstName = "  Maria  ",
                FirstLastName = "  Gomez  ",
                SecondLastName = "  Ruiz  ",
                UserRole = "  Administrator  "
            };

            AppUser? updatedUser = null;

            _userRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.GetByNationalIdAsync("123456789")).ReturnsAsync((AppUser?)null);
            _userRepositoryMock.Setup(r => r.GetByBusinessEmailAsync("new.user@ucr.ac.cr")).ReturnsAsync((AppUser?)null);
            _userRepositoryMock.Setup(r => r.UpdateUserAsync(It.IsAny<AppUser>()))
                .Callback<AppUser>(user => updatedUser = user)
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateUserAsync(userId, request);

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Usuario actualizado correctamente."));
            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser!.FirstName, Is.EqualTo("Maria"));
            Assert.That(updatedUser.FirstLastName, Is.EqualTo("Gomez"));
            Assert.That(updatedUser.SecondLastName, Is.EqualTo("Ruiz"));
            Assert.That(updatedUser.NationalId, Is.EqualTo("123456789"));
            Assert.That(updatedUser.BusinessEmail, Is.EqualTo("new.user@ucr.ac.cr"));
            Assert.That(updatedUser.Email, Is.EqualTo("new.user@ucr.ac.cr"));
            Assert.That(updatedUser.UserRole, Is.EqualTo("Administrator"));

            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<AppUser>()), Times.Once);
        }

        [Test]
        public void UpdateUser_UserDoesNotExist_ThrowsNotFound()
        {
            var userId = Guid.NewGuid();
            var request = BuildValidRequest();

            _userRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((AppUser?)null);
            _userRepositoryMock.Setup(r => r.GetByBusinessEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser?)null);

            Assert.That(async () => await _service.UpdateUserAsync(userId, request), Throws.TypeOf<ZuliNotFoundException>());

            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<AppUser>()), Times.Never);
        }

        [Test]
        public void UpdateUser_EmailBelongsToAnotherUser_ThrowsValidationError()
        {
            var userId = Guid.NewGuid();
            var existingUser = new AppUser { UserId = userId };
            var otherUser = new AppUser { UserId = Guid.NewGuid(), BusinessEmail = "duplicate@ucr.ac.cr" };
            var request = BuildValidRequest(businessEmail: "duplicate@ucr.ac.cr");

            _userRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.GetByBusinessEmailAsync("duplicate@ucr.ac.cr")).ReturnsAsync(otherUser);

            Assert.That(async () => await _service.UpdateUserAsync(userId, request), Throws.TypeOf<ZuliValidationException>());

            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<AppUser>()), Times.Never);
        }

        [Test]
        public void UpdateUser_SameEmailSameUser_DoesNotFail()
        {
            var userId = Guid.NewGuid();
            var existingUser = new AppUser { UserId = userId };
            var request = BuildValidRequest();

            _userRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.GetByNationalIdAsync("123456789")).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.GetByBusinessEmailAsync("user@ucr.ac.cr")).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.UpdateUserAsync(It.IsAny<AppUser>())).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _service.UpdateUserAsync(userId, request));

            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<AppUser>()), Times.Once);
        }

        [Test]
        public void UpdateUser_InvalidRequest_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var request = new RegisterUserRequestDTO
            {
                NationalId = "123",
                BusinessEmail = "not-an-email",
                FirstName = "",
                FirstLastName = "",
                SecondLastName = "",
                UserRole = "Guest"
            };

            Assert.That(async () => await _service.UpdateUserAsync(userId, request), Throws.TypeOf<ZuliValidationException>());

            _userRepositoryMock.Verify(r => r.GetByUserIdAsync(It.IsAny<Guid>()), Times.Never);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<AppUser>()), Times.Never);
        }

        private static RegisterUserRequestDTO BuildValidRequest(string? businessEmail = null)
        {
            return new RegisterUserRequestDTO
            {
                NationalId = "123456789",
                BusinessEmail = businessEmail ?? "user@ucr.ac.cr",
                FirstName = "Maria",
                FirstLastName = "Gomez",
                SecondLastName = "Ruiz",
                UserRole = "Operator"
            };
        }
    }
}