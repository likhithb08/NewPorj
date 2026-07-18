# LOCPS - Project Overview

## Description
Loan Origination & Credit Processing System (LOCPS) is a comprehensive ASP.NET Core MVC application designed to streamline the loan application lifecycle from submission to disbursement.

## Architecture
- **Framework**: ASP.NET Core 8.0 MVC
- **Data Access**: Entity Framework Core 8.0.11
- **Architecture Pattern**: Layered Architecture (Repository, Service, API, MVC)
- **Authentication**: Claims-based Cookie Authentication
- **Authorization**: Role-based Authorization with custom filter

## Key Features
- Role-based access control (Admin, Loan Officer, Underwriter, Customer)
- Loan application management with workflow states
- KYC verification process
- Credit evaluation with automated scoring
- Loan approval and disbursement tracking
- EMI calculation and repayment tracking
- Audit logging for compliance
- RESTful API layer for integration

## Technology Stack
- .NET 8.0
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity (Password Hashing)
- Repository Pattern
- Service Layer Pattern

## Project Structure
```
LOCPS/
├── API/                    # REST API Controllers
├── Common/                 # Shared utilities (PagedResult, ApiResponse)
├── Constants/              # Application constants (Auth, Roles)
├── DTOs/                   # Data Transfer Objects
├── Data/                   # DbContext and Migrations
├── Enums/                  # Application enumerations
├── Extensions/             # Extension methods
├── Infrastructure/         # Infrastructure services
├── Models/                 # Entity models
├── Repositories/           # Data access layer
├── Roles/                  # Role-specific MVC controllers
├── Services/               # Business logic layer
├── ViewModels/             # View-specific models
└── wwwroot/               # Static assets
```

## Database Schema
The application uses the following main entities:
- User, Role
- LoanProduct
- LoanApplication
- Kyc
- CreditEvaluation
- Approval
- Disbursment
- Emi
- Notification
- Auditlog
- Document

## Getting Started
1. Configure connection string in `appsettings.json`
2. Run `dotnet ef database update` to apply migrations
3. Build and run the application
4. Access the application at the configured URL

## Security Considerations
- Secure cookie authentication with 8-hour session duration
- Password hashing using ASP.NET Core Identity
- Role-based route protection
- Audit logging for sensitive operations
