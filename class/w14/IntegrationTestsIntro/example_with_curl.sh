username=$(uuidgen)
password=1234


curl -v -X POST http://localhost:8000/signup -H "Content-Type: application/json" -d "{\"username\": \"$username\", \"password\": \"$password\"}"