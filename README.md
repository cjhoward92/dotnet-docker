# .NET Core Docker example

This example shows how a .NET Core Web API could be run in Docker. It also will spin up a SQL Server instance with the docker mssql-server-linux image.

## Requirements

You need docker and docker-compose installed. When that is done, just run `docker-compose up` and it should all be good to go. Try making a GET request with Postman or curl to http://localhost:5000/api/values. You should get back some JSON that has Name and Quantity keys-value pairs.

Good luck!