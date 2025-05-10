# DatingApp ❤️

A modern, full-stack dating application built with ASP.NET Core and TypeScript. This project demonstrates best practices in web development, including real-time communication, authentication, and profile management.

## Table of Contents 📚
- [Features](#features-)
- [Technologies](#technologies-)
- [Architecture](#architecture-)
- [Installation](#installation-)
- [Configuration](#configuration-)
- [API Documentation](#api-documentation-)
- [Development](#development-)
- [Contributing](#contributing-)
- [License](#license-)

## Features ✨
- **User Authentication & Authorization** 🔐
  - JWT-based authentication
  - Role-based access control
  - Secure password management
  - Email verification

- **Profile Management** 👤
  - Rich user profiles with photos
  - Profile customization
  - Search and filtering capabilities
  - Like/Dislike functionality

- **Real-time Communication** 💬
  - Live chat using SignalR
  - Online presence tracking
  - Message notifications
  - Read receipts

- **Matching System** ❤️
  - Advanced matching algorithm
  - Location-based matching
  - Interest-based recommendations
  - Match notifications

## Technologies 🛠️
### Backend
- ASP.NET Core 7.0
- Entity Framework Core
- SQLite (Development) / SQL Server (Production)
- SignalR for real-time features
- JWT Authentication
- AutoMapper for object mapping
- xUnit for testing

### Frontend
- TypeScript
- Angular 15+
- Bootstrap 5
- NgRx for state management
- SignalR client for real-time features

## Architecture 🏗️
The application follows a clean architecture pattern with:
- Domain-driven design
- Repository pattern
- CQRS pattern for complex operations
- Dependency injection
- Middleware for cross-cutting concerns

## Installation 💻
1. Clone the repository
2. Install .NET 7.0 SDK
3. Install Node.js 16+ and npm
4. Install Angular CLI globally:
```bash
npm install -g @angular/cli
```

### Backend Setup
```bash
cd api
dotnet restore
dotnet run
```

### Frontend Setup
```bash
cd client
npm install
ng serve
```

## Configuration ⚙️
### Backend Configuration
- Update `appsettings.json` with your database connection string
- Configure JWT settings in `appsettings.json`
- Set up email service configuration

### Frontend Configuration
- Update API endpoints in `environment.ts`
- Configure SignalR hub URL
- Set up authentication settings

## API Documentation 📖
The API documentation is available at `/swagger` when running in development mode. It includes:
- Authentication endpoints
- User management
- Profile operations
- Messaging system
- Matching functionality

## Development 🚀
### Backend Development
- Follow C# coding standards
- Use dependency injection
- Implement proper error handling
- Write unit tests for new features

### Frontend Development
- Follow Angular style guide
- Use TypeScript best practices
- Implement responsive design
- Write unit tests

## Contributing 🤝
1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License 📄
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
