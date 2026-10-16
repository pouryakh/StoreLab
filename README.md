# StoreLab

A Multi-Vendor Marketplace Backend built with ASP.NET Core Web API and .NET 9, following Clean Architecture principles.

StoreLab is a backend project focused on marketplace business logic, including store management, product inventory, shopping carts, order processing, wallet-based payments, cancellations, returns, and refunds. It also incorporates authentication, role-based authorization, and structured application logging.

## Features

- **Multi-Vendor Marketplace:** Support for multiple stores and store-specific orders.
- **Store Management:** Seller approval workflow for store activation.
- **Product Management:** Product information and inventory management.
- **Shopping Cart & Checkout:** Cart management and checkout workflows.
- **Order Management:** Store orders, order status management, and cancellation rules.
- **Wallet & Payments:** Wallet-based payment workflows and transaction handling.
- **Returns & Refunds:** Return requests and refund processing.
- **Authentication:** JWT-based authentication with access and refresh tokens.
- **Role-Based Authorization:** Restrict access to protected operations based on user roles.
- **Ticket System:** Support for ticket-based communication and issue handling.
- **Validation:** Request validation using FluentValidation.
- **Logging & Monitoring:** Structured logging with Serilog and Seq.

## Tech Stack

| Technology | Purpose |
|---|---|
| C# | Primary programming language |
| .NET 9 | Application platform |
| ASP.NET Core Web API | HTTP API development |
| Clean Architecture | Separation of concerns and dependency management |
| Entity Framework Core | ORM and database access |
| SQL Server | Relational database |
| Dapper | Lightweight data access |
| JWT | Authentication and token-based authorization |
| FluentValidation | Request and input validation |
| Serilog | Structured application logging |
| Seq | Log collection and inspection |

## Architecture

StoreLab follows Clean Architecture principles to separate business rules from infrastructure and delivery concerns.

The solution is organized around the following responsibilities:

- **Domain:** Core business entities, rules, and domain behavior.
- **Application:** Use cases, application services, interfaces, and validation.
- **Infrastructure:** Database access, persistence implementations, and external technical concerns.
- **API:** HTTP endpoints, request handling, authentication configuration, and application startup.

The intended dependency direction keeps business logic independent of infrastructure implementation details.

## Getting Started

### Prerequisites

Install the following tools before running the project:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server) — a local SQL Server instance or another accessible instance
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)
- [Git](https://git-scm.com/downloads)

Seq is optional unless you want to inspect application logs through its UI.

### 1. Clone the Repository

```bash
git clone https://github.com/pouryakh/StoreLab.git
cd StoreLab
```

Open the solution file in Visual Studio, or navigate to the directory containing the solution.

### 2. Configure the Database Connection

Configure the `DefaultConnection` connection string for your own SQL Server instance.

For local development, you can use an appropriate Windows Authentication connection string, for example:

```text
Server=localhost;Database=StoreLabDb;Trusted_Connection=True;TrustServerCertificate=True;
```

This example assumes a locally accessible SQL Server instance and Windows Authentication. Adjust the server name and authentication settings for your environment.

Do not commit database passwords, production connection strings, or other credentials to the repository.

### 3. Configure JWT Settings Securely

Store the JWT signing key outside the committed configuration files.

For local development, use ASP.NET Core User Secrets from the startup project containing `Program.cs`.

In Visual Studio:

1. Right-click the API startup project.
2. Select **Manage User Secrets**.
3. Add the required settings to the generated `secrets.json` file.

Example:

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_RANDOM_SECRET_KEY"
  }
}
```

Generate your own cryptographically random key; do not use the placeholder above or reuse a previously exposed key.

Keep non-sensitive JWT settings, such as issuer, audience, and token expiration values, in the appropriate application configuration file.

Ensure that the application loads User Secrets in the Development environment and that the JWT configuration is correctly bound to `JwtSettings`.

For deployment, provide secrets through an appropriate secret manager or environment variables rather than committing them to source control.

### 4. Apply Database Migrations

If the repository contains Entity Framework Core migrations, apply them to your configured database.

Run the command from the directory containing the relevant project files, adjusting project paths if necessary:

```bash
dotnet ef database update
```

If the EF Core CLI tool is not installed, install it using the official .NET tooling instructions. Ensure the correct startup and migrations projects are selected if they are separate.

### 5. Run the API

In Visual Studio, select the API project as the startup project and run it.

Alternatively, from the API project directory:

```bash
dotnet restore
dotnet build
dotnet run
```

Use the URLs displayed in the console output to access the running API. If Swagger is enabled in the current environment, open its configured Swagger URL to explore the endpoints.

### 6. Configure Seq (Optional)

StoreLab uses Serilog with console logging and Seq.

If Seq is running locally, the configured endpoint can point to:

```text
http://localhost:5341
```

Make sure the Seq instance is available at the configured address. If it is not running, adjust the logging configuration or start Seq according to its official documentation.

## Security Notes

- Never commit JWT signing keys, passwords, API keys, or private certificates.
- Use User Secrets for local development and an appropriate secret-management solution for deployment.
- Keep environment-specific configuration separate from shared configuration.
- Use a strong, randomly generated JWT signing key.
- Review database permissions and connection settings before deployment.
- Do not expose production secrets in logs or error responses.

## Development Status

StoreLab is an actively developed backend project.

Docker support and automated testing are not presented as completed features in this README. Setup instructions may need adjustment as the project evolves.

## Future Improvements

Potential next steps include:

- Docker-based development environment
- Automated unit and integration tests
- Continuous integration workflow
- Expanded API documentation
- Deployment configuration and production hardening

## License

No license has been specified yet. Unless a license is added to this repository, reuse and redistribution are not automatically permitted.
