namespace UsersApp.Tests;

using Moq;
using Npgsql;
using System.Data;

[TestClass]
public class UnitTestUserRepoWithInterface
{

    public void CleanUpUsersTable()
    {
        // Cleanup the users table before each test
        using var connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres");
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM users";
        cmd.ExecuteNonQuery();
        connection.Close();
    }


    [TestInitialize]
    public void Initialize()
    {
        CleanUpUsersTable();
    }


    [TestMethod]
    public void TestGetAllUsersRealDb()
    {
        // Arrange
        var userRepository = new UserRepositoryWithInterface();
        var user = new User { Name = "Jane Doe" };
        var expectedCount = 1;
        userRepository.InsertUser(user);

        // Act
        var result = userRepository.GetAllUsers();

        // Assert
        Assert.AreEqual(expectedCount, result.Count);
    }

    [TestMethod]
    public void TestGetAllUsersWithMock()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User { Id = 1, Name = "John Doe" }
        };

        var mockConnection = new Mock<IDbConnection>();
        var mockCommand = new Mock<IDbCommand>();
        var mockReader = new Mock<IDataReader>();

        // Setup reader to return one row only
        // When someone calls the Database with the query "SELECT id, name FROM users"
        // Then the mocked database will return ID: 1 and Name: "John Doe"
    
        // Counter to track Read() calls for simulating one row
        var readCallCount = 0;
        // Setup Read() to return true once (has data), then false (no more data)
        mockReader.Setup(r => r.Read()).Returns(() => readCallCount++ == 0); 
        // Setup a value of type Int32 in the position 0 of the reader to return user ID: 1
        mockReader.Setup(r => r.GetInt32(0)).Returns(1);
         // Setup a value of type String in the position 1 of the reader to return user NAME: "John Doe"
        mockReader.Setup(r => r.GetString(1)).Returns("John Doe");
        // Setup ExecuteReader() to return our mock reader
        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        // Setup CreateCommand() to return our mock command
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);


        // Create repository with mocked connection for testing
        var userRepository = new UserRepositoryWithInterface(mockConnection.Object);


        // Act
        var result = userRepository.GetAllUsers();

        // Assert
        Assert.AreEqual(expectedUsers.Count, result.Count);
        Assert.AreEqual(expectedUsers[0].Id, result[0].Id);
        Assert.AreEqual(expectedUsers[0].Name, result[0].Name);
    }

}
