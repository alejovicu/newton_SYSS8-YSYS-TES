namespace UsersApp.Tests;

[TestClass]
public class UnitTestUserRepository
{

    [TestMethod]
    public void TestGetAllUsers()
    {
        // Arrange
        var userRepository = new UserRepository();
        var expectedUserCount = 1;

        // Act
        var result = userRepository.GetAllUsers();

        // Assert
        Assert.AreEqual(expectedUserCount, result.Count);
    }


}
