# Quick Setup Instructions

## For the Reviewer

This project is fully containerized and requires **zero configuration**.

### Prerequisites
- Docker Desktop or Docker Engine with Docker Compose

### Run the Project (3 steps)

```bash
# 1. Clone the repository
git clone <repo-url>
cd <repo-name>

# 2. Start everything
docker-compose up --build

# 3. Wait ~30 seconds for SQL Server to initialize
```

### Access Points

Once running (look for "CartAPI started successfully" in logs):

- **API Documentation**: http://localhost:5000/swagger
- **API Base URL**: http://localhost:5000

### What You'll See

1. **Swagger UI**: Interactive API documentation where you can test all endpoints

### Testing the API

1. Open http://localhost:5000/swagger
2. Try the endpoints (authentication required for protected routes)

### Stop the Application

```bash
docker-compose down
```

### Clean Restart (if needed)

```bash
docker-compose down -v
docker-compose up --build
```

---

**Note**: Database migrations run automatically on startup. No manual steps required!
