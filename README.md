This is a simple API for managing contacts and their skills. You need SQL Server2017 to be able to run the solution.

To use the API, after you download the code, open the solution with Visual Studio, run it, register a user and then, go to https://localhost:[yourPortHere]/swagger to see the API documentation. From the documentation page, you can consume the various endpoints, and manage contacts and skills. 

The solution uses Repository Pattern to avoid coupling the web project to the database project implementation. EF Core Migrations take care of database creation and seeds some skills and skill levels in the tables for ease of use. I did not isolate the business logic; it is very scarce, and I did not want to overengineer the project. For the same reason I did not add any data-level validations.

Skill levels are considered "seed" data and are read-only. If you logout, then register and login with another user, you cannot see the first user's contacts, nor their specific skills. You can see skills and skill levels in general but no other user's customer specific data.

The Test project in the solution keeps all the possible flows on the API controllers safe.
