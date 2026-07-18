# LOCPS - Architecture Design

## Layered Architecture

The LOCPS application follows a clean layered architecture pattern:

```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│  (MVC Controllers + API Controllers)│
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│          Service Layer             │
│      (Business Logic + Validation) │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│        Repository Layer            │
│      (Data Access + Queries)       │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│         Data Layer                 │
│    (Entity Framework + DbContext)  │
└─────────────────────────────────────┘
```

## Repository Pattern

### Generic Repository
- `GenericRepository<T>` provides common CRUD operations
- Base class for all specific repositories
- Includes async methods for database operations

### Specific Repositories
- `IUserRepository` - User data access
- `ILoanProductRepository` - Loan product management
- `ILoanApplicationRepository` - Application lifecycle
- `IKycRepository` - KYC verification data
- `ICreditEvaluationRepository` - Credit scoring
- `IApprovalRepository` - Loan approval workflow
- `IDisbursmentRepository` - Disbursement tracking
- `INotificationRepository` - User notifications
- `IAuditLogRepository` - Audit trail

## Service Layer

### Service Responsibilities
- Business logic implementation
- Validation rules
- Transaction coordination
- Notification dispatch
- Audit logging

### Key Services
- `UserService` - User management and authentication
- `LoanProductService` - Product configuration
- `LoanApplicationService` - Application workflow
- `KycService` - KYC verification process
- `CreditEvaluationService` - Credit scoring logic
- `ApprovalService` - Loan approval decisions
- `DisbursementService` - Fund disbursement
- `NotificationService` - User notifications
- `AuditLogService` - Audit trail management
- `EmiService` - EMI calculation and tracking

## API Layer

### RESTful API Design
- Standard HTTP methods (GET, POST, PUT, DELETE)
- Consistent response format using `ApiResult<T>`
- DTOs for data transfer
- Error handling with appropriate status codes

### API Endpoints
- `/api/User` - User management
- `/api/LoanProduct` - Loan product CRUD
- `/api/LoanApplication` - Application management
- `/api/Kyc` - KYC operations
- `/api/CreditEvaluation` - Credit evaluation
- `/api/Approval` - Loan approval

## MVC Layer

### Role-Based Controllers
- **Admin**: ProductController, UserManagementController
- **Loan Officer**: LoanController, KycController, CreditController
- **Underwriter**: ApprovalController, DisbursementController
- **Customer**: CustomerController

### View Organization
- Role-specific view folders under `Roles/`
- Shared views in `Roles/Shared/`
- Custom view location expander for role routing

## Authentication & Authorization

### Authentication Flow
1. User submits credentials via AccountController
2. Password verification using ASP.NET Core Identity
3. Claims-based cookie creation
4. Role claims added to identity
5. Secure cookie with 8-hour expiration

### Authorization
- Custom `LocpsRoleAuthorizeFilter` for route protection
- Role-based route prefix mapping
- Claims-based permission checking
- Fallback to legacy cookie for demo mode

## Dependency Injection

### Service Registration
- Scoped lifetime for repositories and services
- Transient for stateless services
- Singleton for configuration

### Registration Order
1. DbContext
2. Repositories
3. Services
4. Controllers (automatic)

## Data Access Strategy

### Entity Framework Configuration
- SQL Server provider
- Lazy loading disabled
- Decimal precision configuration
- Cascade delete rules
- Index optimization

### Query Optimization
- AsNoTracking for read-only queries
- Include for eager loading
- Projection for specific columns
- Pagination support

## Error Handling

### Service Layer
- Custom `ServiceException` for business errors
- HTTP status code mapping
- Validation error aggregation

### API Layer
- Try-catch blocks in actions
- Consistent error responses
- Logging of exceptions

### MVC Layer
- ModelState validation
- User-friendly error messages
- Redirect on error

## Caching Strategy
- No caching currently implemented
- Consider adding for:
  - Loan products (rarely change)
  - Role definitions (static)
  - User sessions (already in cookies)
