# SAS.IdentityService

**SAS.IdentityService** is a microservice that handles authentication and user identity management within the **Situational Awareness System (SAS)**. It provides secure JWT-based login, role-based access control, and user registration using **ASP.NET Core Identity** and **.NET 9**.

---

## 📁 Project Structure

```
SAS.IdentityService/
│
├── .github/                          # GitHub workflows and issue templates
├── src/
│   ├── SAS.IdentityService.API/      # API host project
│   │   ├── Controllers/              # Auth, User, Role endpoints
│   │   ├── Data/                     # Identity DbContext, configuration
│   │   ├── Entities/                 # Identity domain models (User, Role)
│   │   ├── Models/                   # DTOs (requests/responses)
│   │   ├── Services/                 # Authentication and user management logic
│   │   └── Program.cs                # Entry point, middleware, DI setup
│   │
│   ├── SAS.IdentityService.ApplicationCore/  # Domain logic and contracts
│   │   ├── Abstractions/            # Interfaces, service contracts
│   │   └── GlobalUsings.g.cs
│   │
│   └── SAS.IdentityService.Infrastructure/   # EF Core persistence, services
│       ├── Persistence/             # Db access layer
│       └── GlobalUsings.g.cs
│
├── SAS.IdentityService.sln          # Solution file
├── .gitignore
└── README.md
```

---

## 🚀 Features

* 🔐 JWT Bearer Authentication + Refresh Tokens
* 👥 Role-based Access Control
* 👤 User Creation, Update, Password Management
* 🧱 Modular architecture using ASP.NET Core 9
* 🗃️ EF Core for data persistence
* 🤝 Integrates with SAS platform (event service, notification service)

---

## 🛠️ Technologies

* [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* ASP.NET Core Identity
* Entity Framework Core
* Ardalis.Result (for clean error and success handling)
* SQL Server (default, but adaptable)

---

## 📡 API Endpoints

| Verb   | Endpoint                    | Description                   |
| ------ | --------------------------- | ----------------------------- |
| `POST` | `/api/Auth/login`           | Authenticate user credentials |
| `POST` | `/api/Auth/register`        | Register a new user           |
| `POST` | `/api/Auth/refresh-token`   | Issue a new JWT               |
| `POST` | `/api/Auth/update-password` | Update user password          |
| `GET`  | `/api/Users`                | List all users                |
| `GET`  | `/api/Users/{id}`           | Retrieve user by ID           |
| `POST` | `/api/UserRoles/assign`     | Assign roles to users         |

---

## 📦 Getting Started

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com)
* SQL Server (or compatible database)
* EF Core CLI: `dotnet tool install --global dotnet-ef`

### Setup Instructions

```bash
# Clone the repo
git clone https://github.com/hasankhadd0ur/SAS.IdentityService.git
cd SAS.IdentityService/src/SAS.IdentityService.API

# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update

# Run the service
dotnet run
```

---

## 🔐 Authentication

* Uses **JWT Bearer Tokens** for stateless authentication.
* Supports **Refresh Tokens** for extending sessions.
* Passwords hashed securely via ASP.NET Core Identity.

---

## 🧪 Testing

> ⚠️ Add test projects (unit/integration) under `tests/` folder in future updates.

---

## 🤝 Contributing

We welcome issues and pull requests. Please follow the standard GitHub workflow.

---

## 📜 License

Licensed under the **MIT License**.

---

## 🏢 Project Maintainers

For issues, reach out to the **SAS Core Development Team**.
