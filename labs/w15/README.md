# Lab w15: Integration tests

The goal for this lab is that you can understand the concept of Integration tests and see how can you implement
the same test in different tools like Postman and MSTest.

During the past two classes we have been working on testing a backend in Postman but in this lab we would do the same we did in Postman but in
MSTest.


Tasks for this lab:

1. Create a branch called `w15_lab`
1. Add the Postman collection with all 4 requirements finished in the path: `labs/w15/newman/collection/ShoppingCartAPP.postman_collection.json`
1. Implement the same tests made in Postman but now in MSTest using HTTPClient and assertions.
    files to update: `labs/w15/ShoppingCartAppIntegration.Tests/Product.cs` and `labs/w15/ShoppingCartAppIntegration.Tests/User.cs`
1. Run the tests towards `dev` environment in MSTest using: `dotnet test -s runSettings/dev.runsettings`
1. Run the tests towards `test` environment in MSTest using: `dotnet test -s runSettings/test.runsettings`
1. Send a pull request towards the class repo with the chages made to the Postman collection and MSTest library.