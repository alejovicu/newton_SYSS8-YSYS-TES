# Lab 01: Implement the unit tests for Bookstore APP


Requirements:

For this lab you will need to install the tool `reportgenerator`

```
dotnet tool install -g dotnet-reportgenerator-globaltool
```

## Context about the application
Bookstore is a simple inventory service with Book entities (ISBN, title, author, stock) and a BookstoreInventory manager that adds new books or restocks existing ones, removes copies by ISBN, finds a book by title (case-insensitive), and checks stock quantity; test project verifies these operations.

## For this lab

1. Create a branch called `labw12_01` and switch to it.
1. Run the command `dotnet test --collect:"Code Coverage"` to generate an initial code coverage report.
1. Validate the current coverage of the project by running the command:
   ```
   reportgenerator -reports:'Bookstore.Tests/TestResults/**/coverage.cobertura.xml' -targetdir:coverage-report -reporttypes:Html
   ```
   The coverage report will be generated in `coverage-report/index.html`
   Read the report. The current coverage is around 2%
1. Implement the tests, then from the path `labs/w12/Bookstore`  run them using the command:
    ```bash
    dotnet test
    ```
1. Increase the coverage up to 75% minimum.
1. Push your branch on your branch towards your Repo.
1. Send a pull request from Your Repo towards the class repo with the name: `labw12_01:  [Your Name]`.
