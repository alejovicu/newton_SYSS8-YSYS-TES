# Frontend app


Compile Docker image and run:
```
docker build . -t app-frontend:0.1.0

# if the newwork is not created
docker network create app-network

docker run --rm --name app-frontend --network app-network -p 80:80 app-frontend:0.1.0
```