This is a simple API for managing contacts and their skills.

To use the API, after you download the code and install the solution, run it, register a user and then, go to https://localhost:<yourPortHere>/swagger to see the API documentation.
From the documentation page, you can consume the various endpoints, and manage contacts and skills. Skill levels are considered "seed" data and are read-only.
If you logout, then register with another user, you cannot see the first user's contacts, nor their specific skills. You can see skills and skill levels in general but not customer specific data.

The solution uses Repository Pattern to avoid coupling the web project to the database implementation.
I did not isolate the business logic, it is very scarece and I did not want to overengineer the project.

The Test project in the solution tests all possible flows on the API controllers.
