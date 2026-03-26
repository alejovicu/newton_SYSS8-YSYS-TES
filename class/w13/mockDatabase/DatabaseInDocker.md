# PostgreSQL: Create a Database in Docker

In this assignment you will learn different ways to mock

- Pieces of your own code
- External API calls
- Database calls


## Database

Create Database:

```shell
docker run --name postgres -p 5432:5432 -e POSTGRES_PASSWORD=mysecretpassword -d postgres
```

Login into the database and create the table:
```shell
docker exec -it postgres-test psql -U testuser -d testdb
```

```SQL
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL
);
```


If you want to create data you can:
```SQL
INSERT INTO users (name) VALUES
('Alice'),
('Bob'),
('Charlie');
```

Validate the data
```SQL
SELECT * FROM users;
```
