# Mocking: Excercise in Class

Let's Start

1. Create a PostgreSQL database container with a table that allows you to store this information of a Product:

    - Name: String
    - Category: String (Supported Values: Tech, Food, Beauty, Health)
    - Price: String

    Example:

    |Name|Category|Price|
    |---|---|---|
    |iPhone 17 pro|Tech|13000|


1. Populate the table with at least 3 products from different categories.

1. Create a Library called `ProductManager` and create the unit tests project `ProductManager.Tests`

1. Create an integration test that calls the library function `GetProductsByCategory` that given a string argument category, it will return a list of products that matches the category.
    
    Use the attribute from MSTest to mark this test as an Integration test
    `[TestCategory("Integration")]`

1. In the library project `ProductManager` implement the function `GetProductsByCategory`.
    **The logic to filter the products by category must be implemented on C# and not in SQL**

1. Run the integration test. It should pass even if you run it several times.

    To run the test use:
    `dotnet test --filter TestCategory=Integration`

1. Create a unit test to validate `GetProductsByCategory`

    Use the attribute from MSTest to mark this test as a Unit test
    `[TestCategory("UnitTest")]`

1. Stop the database container using the command `docker stop [container name]`

1. Run the unit test. It should pass even if you run it several times.

    To run the test use:
    `dotnet test --filter TestCategory=UnitTest`