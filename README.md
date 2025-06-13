# SAS.IdentityService

**SAS.IdentityService** is a microservice responsible for authentication and user management within the **Situational Awareness System (SAS)**. It provides JWT-based authentication, role-based authorization, and user identity management using ASP.NET Core Identity.

---

## 📁 Project Structure

```bash
SAS.IdentityService/
│
├── .github/                      # GitHub configuration files
├── src/
│   └── SAS.IdentityService.API/
│       ├── Abstraction/          # Service interfaces and result models
│       ├── Controllers/          # API endpoints (Auth, User, Role)
│       ├── Data/                 # DbContext and EF configurations
│       ├── Entities/             # Identity-related entities (User, Role, TokenInfo)
│       ├── Migrations/           # EF Core migration history
│       ├── Models/               # Request/response DTOs
│       ├── Services/             # Service implementations
│       ├── Program.cs            # Startup and configuration
│       └── ...                   # Other build and output folders
├── SAS.IdentityService.sln      # Solution file
├── .gitignore
└── .gitattributes
````

---

## 🚀 Features

* ✅ **JWT Authentication & Token Refresh**
* 🔐 **Role-Based Authorization**
* 👤 **User and Role Management**
* 📦 Built using ASP.NET Core 9 and Entity Framework Core
* 📊 Integration-ready for the SAS event platform

---

## 🛠️ Technologies

* [.NET 9](https://dotnet.microsoft.com)
* ASP.NET Core Identity
* Entity Framework Core
* Ardalis.Result (for functional-style result handling)

---

## 🧪 Endpoints Overview

| HTTP Verb | Endpoint                    | Description            |
| --------- | --------------------------- | ---------------------- |
| `POST`    | `/api/Auth/login`           | Login with credentials |
| `POST`    | `/api/Auth/register`        | Register a new user    |
| `POST`    | `/api/Auth/refresh-token`   | Refresh JWT            |
| `GET`     | `/api/Users`                | Get all users          |
| `GET`     | `/api/Users/{id}`           | Get user by ID         |
| `POST`    | `/api/UserRoles/assign`     | Assign roles to user   |
| `POST`    | `/api/Auth/update-password` | Update user password   |

---

## 📦 Getting Started

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* SQL Server (or SQLite, PostgreSQL)
* EF CLI: `dotnet tool install --global dotnet-ef`

### Setup

```bash
git clone https://github.com/your-org/SAS.IdentityService.git
cd SAS.IdentityService/src/SAS.IdentityService.API

# Restore packages
dotnet restore

# Apply EF migrations
dotnet ef database update

# Run the service
dotnet run
```

---

## 🔒 Authentication

* Uses JWT Bearer tokens for secure API access.
* A `refresh token` system is implemented for renewing JWTs securely.

---
## 🧑‍💻 Contributing

Feel free to fork the repo and submit pull requests. Issues and feature requests are welcome.

---

## 📄 License

This project is licensed under the MIT License.

---

## 📬 Contact

For inquiries, reach out to the project maintainer or the SAS core development team.

```

