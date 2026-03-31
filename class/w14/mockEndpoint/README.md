# Rick and Morty API

A .NET library that calls the [Rick and Morty API](https://rickandmortyapi.com/) character endpoint and deserializes responses into strongly-typed `Character` objects with properties: `id`, `Name`, `species`, and `CreatedAt`.


### Unit Tests Only (Mocked)
```bash
dotnet test --filter "TestCategory=Unit"
```

### Integration Tests Only (Real API)
```bash
dotnet test --filter "TestCategory=Integration"
```