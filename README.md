# User Management Solution

This project is built using **.NET 8 MVC** and **Entity Framework Core 8**. It demonstrates a **User Management System** with user registration, login, and CRUD operations. The database is managed using **EF Core** in a **code-first approach**.

---

## Table of Contents

1. [Overview](#overview)  
2. [Technologies Used](#technologies-used)  
3. [Entity Framework Core](#entity-framework-core)  
4. [Database Configuration](#database-configuration)  
5. [Migrations](#migrations)  
6. [Commands Used](#commands-used)  
7. [Adding New Properties](#adding-new-properties)  
8. [Usage](#usage)  
9. [References](#references)  

---

## Overview

This project manages users with properties like:

- `UserName`  
- `Email`  
- `FirstName`  
- `LastName`  

The database schema is automatically created and updated using **Entity Framework Core Migrations**.

---

## Technologies Used

- **.NET 8 MVC**  
- **Entity Framework Core 8**  
- **SQL Server / LocalDB**  
- **Visual Studio 2022**  
- **NuGet Packages**:  
  - Microsoft.EntityFrameworkCore  
  - Microsoft.EntityFrameworkCore.SqlServer  
  - Microsoft.EntityFrameworkCore.Tools  

---

## Entity Framework Core

**Entity Framework Core (EF Core)** is an Object Relational Mapper (ORM) that allows .NET developers to interact with a database using **C# classes** instead of writing SQL.  

### Key Concepts:

- **DbContext**: Represents a session with the database.  
- **DbSet<TEntity>**: Represents a collection of entities of type `TEntity`.  
- **Migrations**: Manage database schema changes.  

---

## Database Configuration

1. Install EF Core packages:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
Configure the connection string in appsettings.json:

json
Copy code
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UserManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Register DbContext in Program.cs:

csharp
Copy code
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
Migrations
Migrations allow EF Core to create or update the database schema based on your models.

Steps:
Add Initial Migration:

bash
Copy code
dotnet ef migrations add InitialCreate --project ./UserManagmentSolution
Update Database:

bash
Copy code
dotnet ef database update --project ./UserManagmentSolution
This will create the database with all tables defined in your DbContext.

Commands Used
Command	Description
dotnet ef migrations add InitialCreate --project ./UserManagmentSolution	Creates the first migration based on your models.
dotnet ef database update --project ./UserManagmentSolution	Applies all pending migrations to the database.
dotnet ef migrations add AddFirstAndLastNameToUsers --project ./UserManagmentSolution	Adds a new migration after updating the User model to include FirstName and LastName.

Adding New Properties
If you want to extend your User model:

Update your User class:

csharp
Copy code
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; } // new property
    public string LastName { get; set; }  // new property
}
Add a migration:

bash
Copy code
dotnet ef migrations add AddFirstAndLastNameToUsers --project ./UserManagmentSolution
Update the database:

bash
Copy code
dotnet ef database update --project ./UserManagmentSolution
Usage
Clone the repository:

bash
Copy code
git clone <your-repo-url>
cd UserManagmentSolution
Install dependencies (if any):

bash
Copy code
dotnet restore
Run the project:

bash
Copy code
dotnet run --project ./UserManagmentSolution
The database will be automatically created and updated according to your migrations.

References
Entity Framework Core Documentation

Code-First Migrations

ASP.NET Core MVC Overview
