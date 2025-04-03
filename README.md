# Shortener

## Overview
Shortener is a high-performance, easy-to-use, and free URL shortening service built with **.NET Core**, **MongoDB**, **Redis**, and **YARP Reverse Proxy**. It allows users to create and manage short URLs, track click statistics, and integrate seamlessly with other applications through a RESTful API. This solution is perfect for anyone needing a reliable, self-hosted URL shortener with advanced monitoring and logging.

## Features
✅ **Fast URL Shortening** – Uses Redis for quick access and caching.
✅ **YARP Reverse Proxy** – Efficient API gateway and load balancing.
✅ **Traditional Controllers & RESTful API** – Ensures better structure and maintainability.
✅ **Logging & Monitoring** – Integrated with **Serilog**, planned **APM, Kibana, Logstash, and ELK** for advanced tracking.
✅ **Unit Tests** – Ensures reliability and code quality.
✅ **User Authentication (Planned)** – Users will be able to manage their links.
✅ **Dashboard for URL Management (Planned)** – A web-based UI to manage links.
✅ **CI/CD Pipeline (Planned)** – Automate testing and deployment.
✅ **Docker Compose (Planned)** – Simplify deployment with containerized services.

## Tech Stack
- **Backend:** .NET Core with RESTful API
- **Database:** MongoDB
- **Caching:** Redis
- **API Gateway:** YARP Reverse Proxy
- **Logging:** Serilog, with planned integration for monitoring tools such as Kibana, Logstash, ELK, or APM.
- **Testing:** Unit Tests
- **CI/CD:** GitHub Actions (Planned)
- **Deployment:** Docker Compose (Planned)

## Installation & Setup

### Prerequisites
- .NET SDK (Latest version)
- Redis Server (Locally or via Docker)
- MongoDB (Locally or via Cloud)
- YARP Reverse Proxy configured

### Clone the Repository
```sh
 git clone https://github.com/Edrisym/Shortener.git
 cd Shortener
```

### Configure Redis
Ensure Redis is running on the expected port (default: `6379`). If using Docker:
```sh
docker run --name redis -d -p 6379:6379 redis:latest
```

### Configure MongoDB
Ensure MongoDB is running locally or on a cloud service (like MongoDB Atlas).

### Run the Project
```sh
dotnet run
```

## API Endpoints

### Shorten a URL
```http
POST /shorten
{
    "url": "https://example.com"
}
```
**Response:**
```json
{
    "shortUrl": "http://yourdomain.com/abc123"
}
```

### Retrieve a Shortened URL
```http
GET /{shortCode}
```

### List All Shortened URLs
```http
GET /
```

## Planned Improvements
- **User Authentication** – Allow users to manage their shortened URLs.
- **Admin Dashboard** – A UI for managing and monitoring short links.
- **Rate Limiting** – Prevent API abuse using Redis.
- **Health Check Endpoints** – Monitor Redis, MongoDB, and API status.
- **Enhanced Logging & Monitoring** – Integrate **Serilog, APM, Kibana, Logstash, and ELK**.
- **CI/CD Pipeline** – Automate builds and deployments.
- **Docker Compose** – Simplify containerized deployment.

## Contribution
Any contributions are welcome! Feel free to submit **issues** and **merge requests** to improve the project. Let's collaborate and make this an even better service!

## License
MIT License. See `LICENSE` for details.
