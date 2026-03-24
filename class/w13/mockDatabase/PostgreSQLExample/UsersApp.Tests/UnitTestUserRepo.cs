namespace UsersApp.Tests;

[TestClass]
public class UnitTestUserRepository
{
    [TestMethod]
    public void TestGetAllUsers()
    {
        // Arrange
        var userRepository = new UserRepository();
        var expectedUsers = new List<User>();
        expectedUsers.Add(new User
            {
                Id = 1,
                Name = "John Doe"
            });

        // Act
        var result = userRepository.GetAllUsers();

        // Assert
        Assert.AreEqual(expectedUsers.Count, result.Count);
    }

}