# User Authentication & Organization API

This project implements a backend API for user authentication and organization management using .NET Framework and C#. 
It provides endpoints for user registration, login, and organization operations.

## Features

- User registration and authentication
- JWT-based authentication
- Organization creation and management
- PostgreSQL database integration
- Input validation and error handling
- Unit and end-to-end testing with xUnit

## Tech Stack

- Backend: .NET Framework (C#)
- Database: PostgreSQL
- ORM: Entity Framework
- Authentication: JWT
- Testing: xUnit
- Deployment: Heroku

## API Endpoints

### Authentication

- `POST /auth/register`: Register a new user and create a default organization
- `POST /auth/login`: Log in a user

### Users

- `GET /api/users/:id`: Get user details (Protected)

### Organizations

- `GET /api/organisations`: Get all organizations for the logged-in user (Protected)
- `GET /api/organisations/:orgId`: Get a single organization (Protected)
- `POST /api/organisations`: Create a new organization (Protected)
- `POST /api/organisations/:orgId/users`: Add a user to an organization (Protected)

