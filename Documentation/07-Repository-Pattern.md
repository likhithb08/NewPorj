# LOCPS - Repository Pattern Documentation

## Overview
The repository pattern provides an abstraction layer between the business logic and data access, promoting separation of concerns and testability.

## Architecture

```
Service Layer → Repository Interface → Repository Implementation → DbContext → Database
```

## Generic Repository

### GenericRepository<T>
Base repository providing common CRUD operations for all entities.

**Methods**:
- `AddAsync(T entity)` - Add new entity
- `UpdateAsync(T entity)` - Update existing entity
- `DeleteAsync(T entity)` - Delete entity
- `GetByIdAsync(int id)` - Get entity by ID
- `GetAllAsync()` - Get all entities
- `GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)` - Get filtered entities

**Features**:
- Generic type constraint for entity classes
- Async operations for database calls
- DbContext injection via constructor
- Automatic change tracking

### Implementation
```csharp
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
}
```

## Specific Repositories

### IUserRepository
User-specific data access operations.

**Methods**:
- `CreateUserAsync(User user)` - Create new user
- `GetUserByEmailIdAsync(string email)` - Get user by email
- `GetUserByIdAsync(int userId)` - Get user by ID
- `UpdateUserAsync(User user)` - Update user
- `DeleteUserByIdAsync(int userId)` - Delete user
- `GetUsersByRoleAsync(Roles role)` - Get users by role

### ILoanProductRepository
Loan product data access.

**Methods**:
- `CreateLoanProductAsync(LoanProduct product)` - Create product
- `GetLoanProductByIdAsync(int productId)` - Get product by ID
- `GetAllLoanProductAsync()` - Get all products
- `UpdateLoanProductAsync(LoanProduct product)` - Update product
- `DeleteLoanProductAsync(LoanProduct product)` - Delete product

### ILoanApplicationRepository
Loan application data access with advanced queries.

**Methods**:
- `CreateLoanApplicationAsync(LoanApplication application)` - Create application
- `GetLoanApplicationByIdAsync(int applicationId)` - Get application by ID
- `GetWithDetailsAsync(int applicationId)` - Get application with related entities
- `UpdateLoanApplicationAsync(LoanApplication application)` - Update application
- `DeleteLoanApplicationAsync(LoanApplication application)` - Delete application
- `SearchAsync(PagedQuery query, ApplicationStatus? status, int? customerId)` - Search with pagination

**Search Features**:
- Pagination support
- Status filtering
- Customer filtering
- Search term support
- Sorting capability

### IKycRepository
KYC verification data access.

**Methods**:
- `CreateKycAsync(Kyc kyc)` - Create KYC record
- `GetKycByApplicationIdAsync(int applicationId)` - Get KYC by application
- `UpdateKycAsync(Kyc kyc)` - Update KYC
- `GetPendingKycAsync(KycStatus status)` - Get KYC by status

### ICreditEvaluationRepository
Credit evaluation data access.

**Methods**:
- `CreateCreditEvaluationAsync(CreditEvaluation evaluation)` - Create evaluation
- `GetCreditEvaluationByApplicationAsync(int applicationId)` - Get evaluation by application
- `UpdateCreditEvaluationAsync(CreditEvaluation evaluation)` - Update evaluation
- `GetPendingCreditEvaluationAsync(CreditRecommendation recommendation)` - Get by recommendation

### IApprovalRepository
Loan approval data access.

**Methods**:
- `CreateApprovalAsync(Approval approval)` - Create approval
- `GetApprovalByApplicationIdAsync(int applicationId)` - Get approval by application
- `GetAllApprovalAsync()` - Get all approvals

### IDisbursmentRepository
Disbursement data access.

**Methods**:
- `CreateDisbursmentAsync(Disbursment disbursment)` - Create disbursement
- `GetDisbursmentByApplicationIdAsync(int applicationId)` - Get by application
- `UpdateDisbursmentAsync(Disbursment disbursment)` - Update disbursement
- `GetPendingDisbursmentsAsync(DisbursmentStatus status)` - Get by status

### INotificationRepository
Notification data access.

**Methods**:
- `CreateNotificationAsync(Notification notification)` - Create notification
- `GetNotificationByUserIdAsync(int userId)` - Get user notifications
- `MarkAsReadAsync(Notification notification)` - Mark as read

### IAuditLogRepository
Audit log data access.

**Methods**:
- `CreateAsync(Auditlog log)` - Create audit entry
- `GetPagedAsync(PagedQuery query)` - Get paginated logs
- `GetByUserIdAsync(int userId)` - Get logs by user

### IDocumentRepository
Document data access.

**Methods**:
- `CreateAsync(Document document)` - Create document
- `GetByIdWithDetailsAsync(int id)` - Get document with details
- `GetByApplicationIdAsync(int applicationId)` - Get documents by application
- `UpdateAsync(Document document)` - Update document

### IEmiRepository
EMI data access.

**Methods**:
- `CreateAsync(Emi emi)` - Create EMI
- `GetByApplicationIdAsync(int applicationId)` - Get EMI schedule
- `GetByIdAsync(int id)` - Get EMI by ID
- `UpdateAsync(Emi emi)` - Update EMI
- `CreateScheduleAsync(int applicationId, decimal principal, decimal annualRate, int tenureMonths, DateTime startDate)` - Generate schedule

## DbContext Configuration

### AppDbContext
Main database context inheriting from `DbContext`.

**DbSet Properties**:
- `DbSet<User> User`
- `DbSet<Role> Role`
- `DbSet<LoanProduct> LoanProduct`
- `DbSet<LoanApplication> LoanApplication`
- `DbSet<Kyc> Kyc`
- `DbSet<CreditEvaluation> CreditEvaluation`
- `DbSet<Approval> Approval`
- `DbSet<Disbursment> Disbursment`
- `DbSet<Emi> Emi`
- `DbSet<Notification> Notification`
- `DbSet<Auditlog> Auditlog`
- `DbSet<Document> Document`

### OnModelCreating Configuration

**Relationships**:
- Foreign key constraints
- Cascade delete rules
- Navigation properties

**Decimal Precision**:
- Financial fields configured with precision (18,2)

**Indexes**:
- Unique constraints on key fields
- Composite indexes for common queries

## Query Optimization

### AsNoTracking
Used for read-only queries to improve performance:

```csharp
public async Task<IEnumerable<T>> GetAllAsync()
{
    return await _dbSet.AsNoTracking().ToListAsync();
}
```

### Include
Eager loading of related entities:

```csharp
public async Task<LoanApplication?> GetWithDetailsAsync(int id)
{
    return await _context.LoanApplication
        .Include(a => a.Customer)
        .Include(a => a.Product)
        .Include(a => a.Kyc)
        .FirstOrDefaultAsync(a => a.ApplicationId == id);
}
```

### Where Filtering
Dynamic query building:

```csharp
var query = _dbSet.AsQueryable();
if (status.HasValue)
    query = query.Where(a => a.Status == status.Value);
if (customerId.HasValue)
    query = query.Where(a => a.CustomerId == customerId.Value);
```

### Pagination
Efficient pagination with Skip/Take:

```csharp
var items = await query
    .Skip((query.Page - 1) * query.PageSize)
    .Take(query.PageSize)
    .ToListAsync();
```

## Best Practices

1. **Async Operations**: All database calls are async
2. **Dispose Pattern**: DbContext disposed properly via DI
3. **Unit of Work**: Services coordinate multiple repository calls
4. **No Business Logic**: Repositories only handle data access
5. **Interface-Based**: All repositories implement interfaces
6. **Generic Base**: Common operations in GenericRepository
7. **Specific Methods**: Complex queries in specific repositories
8. **Error Handling**: Database exceptions propagated to service layer

## Future Enhancements

1. **Specification Pattern**: Complex query composition
2. **Caching**: Repository-level caching
3. **Bulk Operations**: Bulk insert/update support
4. **Stored Procedures**: Complex data operations
5. **Query Logging**: Log generated SQL
6. **Connection Resilience**: Retry logic for transient failures
7. **Read Replicas**: Separate read/write databases
8. **Soft Delete**: Global soft delete support
