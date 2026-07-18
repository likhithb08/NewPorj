# LOCPS - Database Schema

## Overview
LOCPS uses SQL Server as the database with Entity Framework Core for ORM. The schema is organized around the loan application lifecycle.

## Entity Relationships

```
User (1) ----< (N) LoanApplication
Role (1) ----< (N) User

LoanProduct (1) ----< (N) LoanApplication

LoanApplication (1) ----< (1) Kyc
LoanApplication (1) ----< (1) CreditEvaluation
LoanApplication (1) ----< (1) Approval
LoanApplication (1) ----< (1) Disbursment
LoanApplication (1) ----< (N) Emi
LoanApplication (1) ----< (N) Document

User (1) ----< (N) Notification
User (1) ----< (N) Auditlog
```

## Tables

### User
Stores user account information and authentication data.

| Column | Type | Description |
|--------|------|-------------|
| UserId | int (PK) | Unique identifier |
| UserName | string(50) | Login username |
| Email | string(100) | Email address |
| FullName | string(100) | Full name |
| PasswordHash | string | Hashed password |
| PhoneNumber | string(15) | Contact number |
| RoleId | int (FK) | Role reference |
| IsActive | bool | Account status |
| CreatedDate | datetime | Account creation date |
| LastLoginDate | datetime? | Last login timestamp |

### Role
Defines user roles for authorization.

| Column | Type | Description |
|--------|------|-------------|
| RoleId | int (PK) | Unique identifier |
| RoleName | string(50) | Role name |
| RoleToken | string(50) | Token for claims |
| Description | string(200) | Role description |

### LoanProduct
Configurable loan product definitions.

| Column | Type | Description |
|--------|------|-------------|
| ProductId | int (PK) | Unique identifier |
| ProductName | string(50) | Product name |
| ProductDescription | string(500) | Description |
| MinAmount | decimal | Minimum loan amount |
| MaxAmount | decimal | Maximum loan amount |
| InterestRate | decimal | Annual interest rate |
| MaxTenureMonths | int | Maximum tenure in months |
| ProcessingFee | decimal | Processing fee amount |
| IsActive | bool | Product availability |
| CreatedAt | datetime | Creation timestamp |
| CreatedByUserId | int (FK) | Creator reference |

### LoanApplication
Core entity for loan applications.

| Column | Type | Description |
|--------|------|-------------|
| ApplicationId | int (PK) | Unique identifier |
| ApplicationNumber | string(50) | Application reference |
| CustomerId | int (FK) | Customer reference |
| ProductId | int (FK) | Product reference |
| RequestedAmount | decimal | Requested loan amount |
| ApprovedAmount | decimal? | Approved amount |
| AnnualIncome | decimal | Customer annual income |
| EmploymentType | string(50) | Employment type |
| Status | enum | Application status |
| CreatedAt | datetime | Submission date |
| LastUpdatedDate | datetime? | Last update |
| CreatedByUserId | int (FK) | Creator reference |

### Kyc
Know Your Customer verification data.

| Column | Type | Description |
|--------|------|-------------|
| KycId | int (PK) | Unique identifier |
| ApplicationId | int (FK) | Application reference |
| AadhaarNumber | string(12) | Aadhaar card number |
| PanNumber | string(10) | PAN card number |
| DateOfBirth | datetime | Date of birth |
| Gender | string(10) | Gender |
| AddressProofType | string(50) | Address proof type |
| AddressProofNumber | string(50) | Address proof number |
| IdentityProofType | string(50) | Identity proof type |
| IdentityProofNumber | string(50) | Identity proof number |
| VerificationStatus | enum | KYC status |
| VerifiedByUserId | int? (FK) | Verifier reference |
| VerifiedDate | datetime? | Verification date |
| CreatedDate | datetime | Submission date |
| IsActive | bool | Record status |

### CreditEvaluation
Credit scoring and evaluation results.

| Column | Type | Description |
|--------|------|-------------|
| CreditId | int (PK) | Unique identifier |
| ApplicationId | int (FK) | Application reference |
| EvaluatedByUserId | int (FK) | Evaluator reference |
| EvaluatedDate | datetime | Evaluation date |
| CreditScore | int | Credit score (300-900) |
| DebitToIncomeRatio | decimal | DTI ratio |
| PaymentHistoryScore | int | Payment history score |
| ExistingLiabilities | decimal | Existing liabilities |
| CreditRecommendation | enum | Recommendation |
| Comments | string(500) | Evaluation comments |

### Approval
Loan approval decisions.

| Column | Type | Description |
|--------|------|-------------|
| ApprovalId | int (PK) | Unique identifier |
| ApplicationId | int (FK) | Application reference |
| ApprovedByUserId | int (FK) | Approver reference |
| ApprovedAmount | long? | Approved amount |
| ApprovedTenureMonths | int | Approved tenure |
| ApprovedInterestRate | decimal | Approved rate |
| ApprovalStatus | enum | Approval status |
| ApprovalDate | datetime | Decision date |
| RejectionReason | string(200) | Rejection reason |
| Comments | string(500) | Approval comments |

### Disbursment
Loan disbursement tracking.

| Column | Type | Description |
|--------|------|-------------|
| DisbursmentId | int (PK) | Unique identifier |
| ApplicationId | int (FK) | Application reference |
| AmountApproved | int | Disbursed amount |
| DisbursmentDate | datetime? | Disbursement date |
| DisbursmentMode | enum | Disbursement mode |
| BankAccountNumber | long | Bank account |
| BankName | string(100) | Bank name |
| TransactionId | string(50) | Transaction reference |
| Status | enum | Disbursement status |
| ProcessedByUserID | int (FK) | Processor reference |
| Notes | string(500) | Additional notes |

### Emi
Equated Monthly Installment schedule.

| Column | Type | Description |
|--------|------|-------------|
| EmiId | int (PK) | Unique identifier |
| ApplicationID | int (FK) | Application reference |
| EmiNumber | int | EMI sequence |
| EmiAmount | int | EMI amount |
| DueDate | datetime | Due date |
| PaidDate | datetime? | Payment date |
| PaidAmount | decimal? | Amount paid |
| PenaltyAmount | decimal | Penalty amount |
| Status | enum | Payment status |

### Notification
User notifications.

| Column | Type | Description |
|--------|------|-------------|
| NotificationId | int (PK) | Unique identifier |
| UserId | int (FK) | User reference |
| NotificationType | enum | Notification type |
| Title | string(50) | Notification title |
| Message | string(300) | Notification message |
| RelatedApplicationId | int (FK) | Related application |
| IsRead | bool | Read status |
| CreatedDate | datetime | Creation date |
| ReadDate | datetime | Read timestamp |

### Auditlog
Audit trail for compliance.

| Column | Type | Description |
|--------|------|-------------|
| AuditId | int (PK) | Unique identifier |
| UserId | int (FK) | User reference |
| Actions | enum | Action performed |
| EntityId | string(50) | Entity identifier |
| OldValue | string(50) | Previous value |
| NewValue | string(50) | New value |
| Timestamp | datetime? | Action timestamp |
| IpAddress | string(50) | IP address |
| UserAgent | string(50) | User agent |

### Document
Document storage references.

| Column | Type | Description |
|--------|------|-------------|
| DocumentId | int (PK) | Unique identifier |
| ApplicationId | int (FK) | Application reference |
| DocumentType | string(50) | Document type |
| FileName | string(255) | File name |
| FilePath | string(500) | File path |
| UploadedDate | datetime | Upload timestamp |
| UploadedByUserId | int (FK) | Uploader reference |

## Enums

### ApplicationStatus
- Submitted
- KYCPending
- KYCVerified
- UnderReview
- CreditEvaluated
- Approved
- Rejected
- Disbursed

### KycStatus
- Pending
- Verified
- Rejected

### CreditRecommendation
- Pending
- Approved
- Conditional
- Rejected

### ApprovalStatus
- Pending
- Approved
- Rejected

### DisbursmentStatus
- Pending
- Completed
- Failed

### EmiStatus
- Pending
- Paid
- Overdue

### NotificationType
- ApplicationSubmitted
- ApplicationStatusUpdate
- DocumentRequest
- ApprovalUpdate
- KYCVerified
- CreditEvaluated
- DisbursementProcessed

### Actions (Audit)
- Created
- Updated
- Deleted
- Viewed

## Indexes
- Foreign key indexes on all FK columns
- Unique index on User.Email
- Unique index on LoanApplication.ApplicationNumber
- Composite index on LoanApplication.Status + CreatedAt
