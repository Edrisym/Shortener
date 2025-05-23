
<a name="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

<div align="center">
  <a href="https://github.com/Edrisym/Blink/graphs/contributors"><img src="https://img.shields.io/github/contributors/Edrisym/Blink?style=for-the-badge" alt="Contributors"></a>
  <a href="https://github.com/Edrisym/Blink/network/members"><img src="https://img.shields.io/github/forks/Edrisym/Blink?style=for-the-badge" alt="Forks"></a>
  <a href="https://github.com/Edrisym/Blink/stargazers"><img src="https://img.shields.io/github/stars/Edrisym/Blink?style=for-the-badge" alt="Stargazers"></a>
  <a href="https://github.com/Edrisym/Blink/issues"><img src="https://img.shields.io/github/issues/Edrisym/Blink?style=for-the-badge" alt="Issues"></a>
  <a href="https://github.com/Edrisym/Blink/blob/main/LICENSE"><img src="https://img.shields.io/github/license/Edrisym/Blink?style=for-the-badge" alt="MIT License"></a>
</div>


# Blink

## Overview
Blink is a high-performance, easy-to-use, and free URL shortening service built with **.NET Core**, **MongoDB**, **Redis**, and **YARP Reverse Proxy**. It allows users to create and manage short URLs, track click statistics, and integrate seamlessly with other applications through a RESTful API. This solution is perfect for anyone needing a reliable, self-hosted URL shortener with advanced monitoring and logging.

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
 git clone https://github.com/Edrisym/Blink.git
 cd Blink
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
POST /{longUrl}
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
