# Cart API - .NET Core 8

A RESTful API for managing users, items, and shopping carts, built with .NET Core 8, Entity Framework Core, and JWT authentication. Includes full observability with OpenTelemetry for logging, tracing, and metrics.

## Features

- 🔐 JWT-based authentication
- 🗄️ SQL Server database with Entity Framework Core
- 📊 OpenTelemetry observability (logging, tracing, metrics)
- 🐳 Fully containerized with Docker
- 📝 Swagger/OpenAPI documentation

## Prerequisites

- [Docker](https://www.docker.com/get-started) (version 20.10 or higher)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 1.29 or higher)

**That's it!** No need to install .NET SDK, SQL Server, or any other dependencies locally.

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd <repo-name>
   ```

2. **Start the application**
   ```bash
   docker-compose up --build
   ```

   This will:
   - Build the .NET API
   - Start SQL Server
   - Run database migrations automatically
   - Start the API server

3. **Access the services**
   - **API**: http://localhost:5000
   - **Swagger UI**: http://localhost:5000/swagger

## What's Running?

- **CartAPI** (port 5000): Your .NET Core API
- **SQL Server** (port 1433): Database server

## API Documentation

Once running, visit http://localhost:5000/swagger to explore the API endpoints interactively.

### Authentication

Most endpoints require JWT authentication. To authenticate:

1. Register or login to get a JWT token
2. In Swagger UI, click the "Authorize" button
3. Enter: `Bearer <your-token>`
4. Click "Authorize"

## Observability

### Logs
View application logs in the console where you ran `docker-compose up`, or use:
```bash
docker-compose logs -f webapi
```

### Metrics
Metrics are exported to the console and can be viewed in the application logs.

## Development

### Stopping the Application
```bash
docker-compose down
```

### Stopping and Removing Data
```bash
docker-compose down -v
```

### Rebuilding After Code Changes
```bash
docker-compose up --build
```

### Viewing Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f webapi
```

## Project Structure

```
├── Config/              # Configuration classes
├── Controllers/         # API endpoints
├── Data/                # Database context
├── DTOs/                # Request and Response classes
├── Migrations/          # Migrations
├── Models/              # Data models
├── Services/            # Business logic
├── Dockerfile           # Container definition
├── docker-compose.yml   # Multi-container orchestration
└── Program.cs           # Application entry point
```

## Configuration

Configuration is handled through environment variables in `docker-compose.yml`. Key settings:

- **Database Connection**: Automatically configured to use the SQL Server container
- **JWT Settings**: Pre-configured with default values (change for production!)

## Troubleshooting

### Database Connection Issues
If you see database connection errors, the SQL Server container might still be starting. Wait 30 seconds and try again. The API will retry the connection automatically.

### Port Already in Use
If port 5000 or 1433 is already in use, you can change the ports in `docker-compose.yml`:
```yaml
ports:
  - "5001:8080"  # Change 5000 to 5001
```

### Clean Start
If you encounter issues, try a clean restart:
```bash
docker-compose down -v
docker-compose up --build
```

## Production Considerations

⚠️ **Before deploying to production:**

1. Change the SQL Server SA password
2. Use strong JWT secret keys
3. Store secrets in environment variables or a secret manager
4. Enable HTTPS
5. Configure proper logging and monitoring
6. Review security settings
7. Use proper OTLP exporters (not console)

## Technologies Used

- .NET Core 8.0
- Entity Framework Core
- SQL Server 2022
- OpenTelemetry
- Docker & Docker Compose
- Swagger/OpenAPI

