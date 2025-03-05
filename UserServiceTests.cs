using Moq;
using FluentAssertions;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.DAL.Repositories;


namespace SocialNetwork.Tests 
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<MessageService> _messageServiceMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _messageServiceMock = new Mock<MessageService>();

            _userService = new UserService();
        }

        // Пример теста для метода Register, когда имя пустое
        [Test]
        public void Register_ShouldThrowArgumentNullException_WhenFirstNameIsNull()
        {
            // Arrange
            var userRegistrationData = new UserRegistrationData
            {
                FirstName = null,
                LastName = "Doe",
                Email = "test@test.com",
                Password = "password123"
            };

            Action act = () => _userService.Register(userRegistrationData);

            act.Should().Throw<ArgumentNullException>();
        }

        // Тест для метода AddFriend, когда друг не найден
        [Test]
        public void AddFriend_ShouldThrowUserNotFoundException_WhenFriendNotFound()
        {
            int userId = 1;
            string friendEmail = "nonexistent@test.com";

            _userRepositoryMock.Setup(repo => repo.FindByEmail(friendEmail)).Returns((UserEntity)null);

            Action act = () => _userService.AddFriend(userId, friendEmail);

            act.Should().Throw<UserNotFoundException>();
        }

        // Тест для метода AddFriend, когда друг найден
        [Test]
        public void AddFriend_ShouldCallAddFriend_WhenFriendExists()
        {
            int userId = 1;
            string friendEmail = "friend@test.com";
            var friend = new UserEntity { id = 2, firstname = "Friend", lastname = "User" };

            _userRepositoryMock.Setup(repo => repo.FindByEmail(friendEmail)).Returns(friend);
            _userRepositoryMock.Setup(repo => repo.AddFriend(userId, friend.id)).Verifiable();

            _userService.AddFriend(userId, friendEmail);

            _userRepositoryMock.Verify(repo => repo.AddFriend(userId, friend.id), Times.Once);
        }
    }
}
