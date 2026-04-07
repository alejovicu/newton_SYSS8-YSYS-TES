# Excercise in class: Run the collection in Postman and Newman

1. Redeploy the environments for dev and test to start from a clean state
1. Set the admin user to user: `admin` password: `admin`
1. Import the collection and environment files in your postman workplace
1. Run the collection in Postman
1. Automate the pending requirements to validate
1. Run the collection (after implementation) in Postman
1. Export the Postman collection and overwrite the file `ShoppingCartAPP.postman_collection.json` for this class with yours. 
1. Run the collection from the terminal towards dev environment using Newman
1. Run the collection from the terminal towards test environment using Newman


## Notes

Command to run a collection using newman from this path (`class/w15/newman`):

```
newman run collection/ShoppingCartAPP.postman_collection.json -e env/dev.postman_environment.json
```