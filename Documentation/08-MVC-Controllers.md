# LOCPS - MVC Controllers Documentation

## Overview
MVC controllers handle HTTP requests, coordinate with services, and return views or data. Controllers are organized by role for better separation of concerns.

## Controller Organization

```
Roles/
├── Admin/
│   └── Controllers/
│       ├── ProductController.cs
│       ├── UserManagementController.cs
│       └── ReportsController.cs
├── LoanOfficer/
│   └── Controllers/
│       ├── LoanController.cs
│       ├── KycController.cs
│       ├── CreditController.cs
│       └── DocumentController.cs
├── Underwriter/
│   └── Controllers/
│       ├── ApprovalController.cs
│       └── DisbursementController.cs
├── Customer/
│   └── Controllers/
│       └── CustomerController.cs
└── Shared/
    └── Controllers/
        └── AccountController.cs
```

## Shared Controllers

### AccountController
Handles authentication and user session management.

**Actions**:
- `Login` - Display login form
- `Login(LoginViewModel)` - Process login
- `Register` - Display registration form
- `Register(RegisterViewModel)` - Process registration
- `Logout` - Logout user
- `AccessDenied` - Display access denied page

**Features**:
- Claims-based authentication
- Cookie creation with role claims
- Password hashing with ASP.NET Core Identity
- Role-based redirect after login
- Audit logging for login events

## Admin Controllers

### ProductController
Manages loan product configuration.

**Actions**:
- `Index()` - List all products
- `Create()` - Display create form
- `Create(LoanProduct)` - Process product creation
- `Details(int id)` - Display product details
- `Edit(int id)` - Display edit form
- `Edit(LoanProduct)` - Process product update
- `Delete(int id)` - Display delete confirmation
- `DeleteConfirmed(int id)` - Process deletion

**Service Integration**:
- Uses `ILoanProductService`
- Tracks `CreatedByUserId` from claims
- Validates model state before service call
- Returns to Index on success

### UserManagementController
Manages system users.

**Actions**:
- `Index()` - List all users
- `Create()` - Display create form
- `Create(User)` - Process user creation
- `Edit(int id)` - Display edit form
- `Edit(User)` - Process user update
- `Delete(int id)` - Delete user
- `AssignRole(int userId, int roleId)` - Assign role

**Service Integration**:
- Uses `IUserService`
- Role assignment functionality
- User activation/deactivation

### ReportsController
Generates system reports.

**Actions**:
- `Index()` - Report dashboard
- `LoanApplications()` - Application report
- `Disbursements()` - Disbursement report
- `AuditLogs()` - Audit log report

## Loan Officer Controllers

### LoanController
Manages loan applications.

**Actions**:
- `Index(int page, int pageSize, ApplicationStatus? status)` - List applications with pagination
- `Create()` - Display create form
- `Create(LoanApplication)` - Process application creation
- `Details(int id)` - Display application details
- `Edit(int id)` - Display edit form
- `Edit(LoanApplication)` - Process application update
- `UpdateStatus(int id, ApplicationStatus status)` - Update application status

**Service Integration**:
- Uses `ILoanApplicationService`
- Uses `ILoanProductService` for product dropdown
- Pagination support via `PagedQuery`
- Status filtering
- User ID from claims

### KycController
Manages KYC verification.

**Actions**:
- `Index()` - List pending KYC
- `Details(int id)` - Display KYC details
- `Verify(int id)` - Verify KYC
- `Reject(int id)` - Reject KYC

**Service Integration**:
- Uses `IKycService`
- Status updates trigger notifications
- Audit logging for verification

### CreditController
Manages credit evaluation.

**Actions**:
- `Index()` - List pending evaluations
- `Evaluate(int id)` - Display evaluation form
- `Evaluate(CreditEvaluation)` - Process evaluation
- `Approve(int id)` - Approve evaluation
- `Reject(int id)` - Reject evaluation

**Service Integration**:
- Uses `ICreditEvaluationService`
- Credit score calculation
- DTI ratio calculation

### DocumentController
Manages document uploads.

**Actions**:
- `Index(int applicationId)` - List documents
- `Upload(int applicationId)` - Display upload form
- `Upload(Document)` - Process upload
- `Download(int id)` - Download document
- `Delete(int id)` - Delete document

## Underwriter Controllers

### ApprovalController
Manages loan approvals.

**Actions**:
- `Index()` - List pending approvals
- `Details(int id)` - Display approval details
- `Approve(int id)` - Display approve form
- `Approve(Approval)` - Process approval
- `Reject(int id)` - Display reject form
- `Reject(Approval)` - Process rejection

**Service Integration**:
- Uses `IApprovalService`
- Amount and tenure validation
- Notification on approval/rejection

### DisbursementController
Manages loan disbursements.

**Actions**:
- `Index()` - List pending disbursements
- `Details(int id)` - Display disbursement details
- `Process(int id)` - Display process form
- `Process(Disbursment)` - Process disbursement
- `History()` - Disbursement history

**Service Integration**:
- Uses `IDisbursementService`
- Bank account validation
- Transaction ID generation

## Customer Controllers

### CustomerController
Customer self-service portal.

**Actions**:
- `Index()` - List customer's applications
- `Create()` - Display application form
- `Create(LoanApplication)` - Process application
- `Details(int id)` - Display application details
- `SubmitKyc(int applicationId, Kyc)` - Submit KYC documents

**Service Integration**:
- Uses `ILoanApplicationService`
- Uses `ILoanProductService`
- Uses `IKycService`
- Customer ID from claims
- KYC submission integration

## Controller Best Practices

### Dependency Injection
All services injected via constructor:

```csharp
public class ProductController : Controller
{
    private readonly ILoanProductService _loanProductService;

    public ProductController(ILoanProductService loanProductService)
    {
        _loanProductService = loanProductService;
    }
}
```

### Async Actions
All database operations use async:

```csharp
public async Task<IActionResult> Index()
{
    var products = await _loanProductService.GetAllAsync(true);
    return View(products);
}
```

### Model Validation
Validate model state before service call:

```csharp
if (!ModelState.IsValid)
    return View(product);

try
{
    await _loanProductService.CreateAsync(product, createdByUserId);
    return RedirectToAction(nameof(Index));
}
catch (Exception ex)
{
    ModelState.AddModelError(string.Empty, ex.Message);
    return View(product);
}
```

### User Context
Extract user information from claims:

```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var createdByUserId = int.TryParse(userId, out var id) ? id : 1;
```

### ViewBag for Dropdowns
Pass reference data via ViewBag:

```csharp
var products = await _loanProductService.GetAllAsync(true);
ViewBag.Products = products;
```

### Error Handling
Try-catch with user-friendly messages:

```csharp
catch (ServiceException ex)
{
    ModelState.AddModelError(string.Empty, ex.Message);
    return View(model);
}
```

## View Location

### Role-Based View Location
Custom `LocpsViewLocationExpander` enables role-specific views:

```
Views/
├── Admin/
│   ├── Product/
│   │   ├── Index.cshtml
│   │   └── Create.cshtml
│   └── UserManagement/
├── LoanOfficer/
│   ├── Loan/
│   └── Kyc/
├── Underwriter/
│   ├── Approval/
│   └── Disbursement/
├── Customer/
│   └── Customer/
└── Shared/
    ├── Account/
    └── Layout/
```

### Route Prefix
Controllers automatically route to role-specific prefix based on folder structure.

## Authorization

### Role-Based Authorization
Custom filter `LocpsRoleAuthorizeFilter` enforces role access:

```csharp
[ServiceFilter(typeof(LocpsRoleAuthorizeFilter))]
public class ProductController : Controller
{
    // Only accessible by Admin role
}
```

### Route Permission Mapping
Filter checks route prefix against role permissions:

```csharp
private static readonly Dictionary<string, HashSet<string>> RoutePermissions = new()
{
    { "Admin", new HashSet<string> { "Product", "UserManagement" } },
    { "LoanOfficer", new HashSet<string> { "Loan", "Kyc", "Credit" } },
    // ...
};
```

## Future Enhancements

1. **API Controllers**: Add more API endpoints
2. **View Components**: Reusable view components
3. **Tag Helpers**: Custom tag helpers
4. **Partial Views**: Modular view components
5. **Client-Side Validation**: jQuery Validation integration
6. **AJAX Forms**: Unobtrusive AJAX
7. **File Upload**: Better file handling
8. **Export**: Excel/PDF export functionality
