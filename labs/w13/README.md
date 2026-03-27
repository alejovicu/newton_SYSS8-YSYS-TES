# Lab w13: Mocking a DB connection

* Continue with the excercise made in class w13: class/w13/mockDatabase/ClassExcercise/README.md
* In the Unit Test create a mocked DB response which contains two or more rows with products for different categories, so you can verify if the filter by category is working as expected
* Send a Pull request to the class repo with the PR Name: `Lab w13 - [your name]`



## Hint

This is an example of you can create a mocked response like:

|Col 0| Col 1|
|---|---|
|1|John Doe|
|2|Jane Doe|


```C#
    // Arrange
    var mockConnection = new Mock<IDbConnection>();
    var mockCommand = new Mock<IDbCommand>();
    var mockReader = new Mock<IDataReader>();

        
    mockReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

    // Set up first row values
    mockReader.SetupSequence(r => r.GetInt32(0))
                .Returns(1) 
                .Returns(2);

    mockReader.SetupSequence(r => r.GetString(1))
                 .Returns("John Doe")
                .Returns("Jane Doe");


    mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
    mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

    //Setting the DB as source
    var userRepository = new UserRepositoryWithInterface(mockConnection.Object);
```