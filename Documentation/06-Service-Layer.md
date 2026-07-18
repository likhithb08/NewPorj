# LOCPS - Service Layer Documentation

## Overview
The service layer contains business logic, validation rules, and coordinates operations between controllers and repositories. All services are registered with scoped lifetime in DI container.

## Service Architecture

```
Controller → Service → Repository → Database
```

## Service List

### UserService
**Purpose**: User management and authentication support

**Methods**:
- `RegisterUserAsync(UserCreateViewModel)` - Register new user with password hashing
- `LoginAsync(string email, string password)` - Validate credentials and return user
- `GetUserByIdAsync(int userId)` - Get user by ID
- `GetAllUsersAsync()` - Retrieve all users
- `GetUsersByRoleAsync(Roles role)` - Get users by role
- `UpdateUserAsync(UserUpdateViewModel)` - Update user details
- `DeleteUserAsync(int userId)` - Soft delete user
- `ChangePasswordAsync(int userId, string oldPassword, string newPassword)` - Change user password
- `AssignRoleAsync(int userId, int roleId)` - Assign role to user

**Business Rules**:
- Email must be unique
- Password hashed using ASP.NET Core Identity
- Inactive users cannot login
- Audit logging for all user operations

### LoanProductService
**Purpose**: Loan product configuration and management

**Methods**:
- `CreateAsync(LoanProduct product, int createdByUserId)` - Create new loan product
- `GetByIdAsync(int id)` - Get product by ID
- `GetAllAsync(bool activeOnly)` - Get all products (optionally active only)
- `UpdateAsync(LoanProduct product)` - Update product details
- `DeleteAsync(int id)` - Soft delete product (set IsActive = false)

**Business Rules**:
- Products cannot be deleted, only deactivated
- Audit logging for product changes
- CreatedByUserId tracked for accountability

### LoanApplicationService
**Purpose**: Loan application lifecycle management

**Methods**:
- `CreateAsync(LoanApplication application)` - Submit new application
- `GetByIdAsync(int id)` - Get application with details
- `SearchAsync(PagedQuery query, ApplicationStatus? status, int? customerId)` - Search applications
- `UpdateAsync(LoanApplication application)` - Update application
- `DeleteAsync(int id)` - Delete application
- `UpdateStatusAsync(int applicationId, ApplicationStatus status, int actorUserId)` - Update application status
- `GenerateApplicationNumber()` - Generate unique application number

**Business Rules**:
- Application number format: `LOC-yyyyMMdd-XXXX`
- Amount must be within product limits
- Status changes trigger notifications
- Audit logging for status transitions

### KycService
**Purpose**: KYC verification process management

**Methods**:
- `SubmitAsync(Kyc kyc)` - Submit KYC documents
- `GetByApplicationIdAsync(int applicationId)` - Get KYC by application
- `VerifyAsync(int kycId, int verifiedByUserId)` - Verify KYC documents
- `RejectAsync(int kycId, int verifiedByUserId)` - Reject KYC documents

**Business Rules**:
- KYC status: Pending → Verified/Rejected
- Verification triggers notification to customer
- Audit logging for verification actions

### CreditEvaluationService
**Purpose**: Credit scoring and evaluation

**Methods**:
- `CalculateAndSaveAsync(int applicationId, int evaluatedByUserId)` - Calculate credit score
- `GetByApplicationIdAsync(int applicationId)` - Get evaluation by application
- `ApproveAsync(int applicationId, int userId, string? comments)` - Approve credit evaluation
- `RejectAsync(int applicationId, int userId, string? comments)` - Reject credit evaluation

**Business Rules**:
- Credit score range: 300-900
- DTI ratio calculated automatically
- Score based on income and employment
- Audit logging for evaluation decisions

**Credit Score Calculation**:
```
Base Score: 600
Income > 500000: +100
Income > 300000: +50
Maximum: 900
```

**DTI Calculation**:
```
DTI = (Requested Amount / (Annual Income * 12)) * 100
```

### ApprovalService
**Purpose**: Loan approval workflow

**Methods**:
- `ApproveLoanAsync(int applicationId, int approverUserId, decimal approvedAmount, int tenureMonths, decimal interestRate, string? comments)` - Approve loan
- `RejectLoanAsync(int applicationId, int approverUserId, string reason, string? comments)` - Reject loan
- `GetByApplicationIdAsync(int applicationId)` - Get approval by application
- `GetHistoryAsync()` - Get approval history

**Business Rules**:
- Approval updates application status
- Notification sent to customer
- Audit logging for approval decisions
- Rejection reason required

### DisbursementService
**Purpose**: Loan disbursement management

**Methods**:
- `CreateAsync(Disbursment disbursement)` - Create disbursement record
- `GetByApplicationIdAsync(int applicationId)` - Get disbursement by application
- `GetHistoryAsync()` - Get disbursement history
- `ProcessAsync(int disbursementId, int processedByUserId)` - Process disbursement

**Business Rules**:
- Status: Pending → Completed
- Application status updated to Disbursed
- Notification sent to customer
- Audit logging for disbursement

### NotificationService
**Purpose**: User notification management

**Methods**:
- `CreateAsync(int userId, NotificationType type, string title, string message, int? relatedApplicationId)` - Create notification
- `GetForUserAsync(int userId, bool unreadOnly)` - Get user notifications
- `MarkAsReadAsync(int notificationId)` - Mark notification as read

**Business Rules**:
- Notifications created for key events
- Unread filter available
- Related application tracked

### AuditLogService
**Purpose**: Audit trail management

**Methods**:
- `LogAsync(int userId, Actions action, string entityId, string? oldValue, string? newValue, string? ipAddress, string? userAgent)` - Log audit entry
- `GetPagedAsync(PagedQuery query)` - Get paginated audit logs

**Business Rules**:
- All critical operations logged
- Old and new values tracked
- IP and user agent captured

### EmiService
**Purpose**: EMI calculation and repayment tracking

**Methods**:
- `GenerateScheduleAsync(int applicationId, decimal principal, decimal annualRate, int tenureMonths)` - Generate EMI schedule
- `GetByApplicationIdAsync(int applicationId)` - Get EMI schedule
- `RecordPaymentAsync(int emiId, decimal paidAmount)` - Record EMI payment

**Business Rules**:
- EMI calculated using standard formula
- Schedule generated on approval
- Payments tracked with penalties

**EMI Calculation**:
```
EMI = P * r * (1+r)^n / ((1+r)^n - 1)
Where:
P = Principal
r = Monthly interest rate (annual/12/100)
n = Tenure in months
```

## Error Handling

All services use custom `ServiceException` for business errors:

```csharp
public class ServiceException : Exception
{
    public int StatusCode { get; }
    public ServiceException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}
```

Common status codes:
- `400` - Validation error
- `401` - Unauthorized
- `403` - Forbidden
- `404` - Not found
- `409` - Conflict (duplicate)
- `500` - Internal error

## Validation

### Service-Level Validation
- Input parameter validation
- Business rule validation
- Data consistency checks
- Permission verification

### Model-Level Validation
- Data annotations on models
- Required fields
- String length constraints
- Range validation
- Regular expression patterns

## Transaction Management

Services coordinate transactions implicitly through:
- DbContext unit of work
- Automatic rollback on exception
- Commit on successful operation

## Dependency Injection

All services registered as scoped:

```csharp
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanProductService, LoanProductService>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
// ... other services
```

## Best Practices

1. **Async/Await**: All database operations are async
2. **Null Checks**: Validate inputs before processing
3. **Audit Logging**: Log all critical operations
4. **Notifications**: Trigger notifications for key events
5. **Error Handling**: Use ServiceException for business errors
6. **Validation**: Validate at service layer, not just controller
7. **Separation of Concerns**: Services don't reference HTTP concepts
8. **Testability**: Services are easily testable with mocks

## Future Enhancements

1. **Caching**: Cache frequently accessed data
2. **Events**: Implement event-driven architecture
3. **Background Jobs**: Process long-running operations
4. **Validation**: Add FluentValidation
5. **Transactions**: Explicit transaction control
6. **Retry Logic**: Add retry for transient failures
7. **Metrics**: Add performance monitoring
8. **Logging**: Structured logging with Serilog
