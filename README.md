# Library System

## Description
An API for managing a library system. Can do CRUD operations on Books, Users, Checkouts, Authors, Genre, Publishers, BookCopies, and BookGenres. Its nothing fancy but I tried my best in making it as real life applicable as I could make it.

## Tools
The project is written in C# using ASP.Net Core Web API. It also uses Microsoft SQL Server for database related tasks.

## Running
- Install proper software (.net10 SDK or runtime, Sql Server)
- Create .env file (Example of variables are included in ExampleEnv.txt file)
- Use migration file to create tables in database (need ef cli tools installed for command):
    - dotnet ef database update
- Command to run (dotnet cli):
    -   dotnet run

## Security
The api uses Json Web Tokens to authenticate users. Users can only be created by authenticated users with "admin" or "employee" roles. Note that only "admin" users are able to add/edit roles of a users. This is to prevent privilage escalation.

## First Time Running
If there is no Admin user in the database on startup then one will automatically be created using environement variables in the .env file. This is neccessary since most endpoints (including the one to make users) have authentication checks, so an initial Admin user will be needed to get started. Also passwords are hashed by bcrypt library which will make it more challenging to create the users outside of the application.


## Enpoints by Role
### None
The endpoints that dont require any authentication is getting all/byId Books and Authors; as well as logging in.

### All UserTypes
The only endpoint available to all authenticated users are the one to get checkouts of currently authenticated user.

### Employee
Employees have access to all endpoints, with some endpoints having a few limits (cant create/edit users with a specific role)

### Admin
Access all endpoints.