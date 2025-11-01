# Library System

## Description
An API for managing a library system. Can do CRUD operations on Books, Users, Checkouts, Authors, Genre, Publishers, BookCopies, and BookGenres.

## Tools
The project is written in C# using ASP.Net Core Web API. It also uses Microsoft SQL Server for database related tasks.

## Running
- Install proper software (.net SDK or runtime, Sql Server)
- Create .env file with database connection
- Use migration file to create tables in database (need ef cli tools installed for command):
    - dotnet ef database update
- Command to run (dotnet cli):
    -   dotnet run

