# LOCPS - API Documentation

## Base URL
```
https://localhost:5001/api
```

## Response Format
All API responses follow the `ApiResult<T>` format:

```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": []
}
```

## Authentication
Currently uses cookie-based authentication. Future versions will support JWT tokens.

## Endpoints

### User API

#### Register User
```http
POST /api/User/register
Content-Type: application/json

{
  "userName": "johndoe",
  "email": "john@example.com",
  "fullName": "John Doe",
  "phoneNumber": "1234567890",
  "password": "SecurePass123",
  "roleId": 1
}
```

#### Login
```http
POST /api/User/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123"
}
```

#### Get User
```http
GET /api/User/{id}
```

#### Get All Users
```http
GET /api/User
```

#### Update User
```http
PUT /api/User/{id}
Content-Type: application/json

{
  "userId": 1,
  "userName": "johndoe",
  "email": "john@example.com",
  "fullName": "John Doe",
  "phoneNumber": "1234567890",
  "roleId": 1,
  "isActive": true
}
```

#### Delete User
```http
DELETE /api/User/{id}
```

### Loan Product API

#### Create Product
```http
POST /api/LoanProduct
Content-Type: application/json

{
  "productName": "Home Loan",
  "productDescription": "Home loan for new purchases",
  "minAmount": 100000,
  "maxAmount": 5000000,
  "interestRate": 8.5,
  "maxTenureMonths": 240,
  "processingFee": 5000
}
```

#### Get Product
```http
GET /api/LoanProduct/{id}
```

#### Get All Products
```http
GET /api/LoanProduct?activeOnly=true
```

#### Update Product
```http
PUT /api/LoanProduct/{id}
Content-Type: application/json

{
  "productId": 1,
  "productName": "Home Loan",
  "productDescription": "Updated description",
  "minAmount": 100000,
  "maxAmount": 6000000,
  "interestRate": 8.0,
  "maxTenureMonths": 300,
  "processingFee": 5000,
  "isActive": true
}
```

#### Delete Product
```http
DELETE /api/LoanProduct/{id}
```

### Loan Application API

#### Create Application
```http
POST /api/LoanApplication
Content-Type: application/json

{
  "customerId": 1,
  "productId": 1,
  "requestedAmount": 500000,
  "annualIncome": 600000,
  "employmentType": "Salaried"
}
```

#### Get Application
```http
GET /api/LoanApplication/{id}
```

#### Search Applications
```http
GET /api/LoanApplication/search?pageNumber=1&pageSize=10&searchTerm=&status=&customerId=
```

Query Parameters:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)
- `searchTerm` (string, optional)
- `status` (ApplicationStatus enum, optional)
- `customerId` (int, optional)

#### Update Status
```http
PUT /api/LoanApplication/{id}/status
Content-Type: application/json

{
  "applicationId": 1,
  "status": "Approved"
}
```

#### Delete Application
```http
DELETE /api/LoanApplication/{id}
```

### KYC API

#### Submit KYC
```http
POST /api/Kyc
Content-Type: application/json

{
  "applicationId": 1,
  "aadhaarNumber": "123456789012",
  "panNumber": "ABCDE1234F",
  "dateOfBirth": "1990-01-01",
  "gender": "Male",
  "addressProofType": "Aadhaar",
  "addressProofNumber": "123456789012",
  "identityProofType": "PAN",
  "identityProofNumber": "ABCDE1234F"
}
```

#### Get KYC by Application
```http
GET /api/Kyc/application/{applicationId}
```

#### Verify KYC
```http
PUT /api/Kyc/{id}/verify
```

#### Reject KYC
```http
PUT /api/Kyc/{id}/reject
```

### Credit Evaluation API

#### Calculate Credit Evaluation
```http
POST /api/CreditEvaluation/calculate
Content-Type: application/json

{
  "applicationId": 1,
  "evaluatedByUserId": 2
}
```

#### Get Credit Evaluation
```http
GET /api/CreditEvaluation/application/{applicationId}
```

#### Approve Credit Evaluation
```http
PUT /api/CreditEvaluation/approve
Content-Type: application/json

{
  "applicationId": 1,
  "userId": 2,
  "comments": "Good credit score"
}
```

#### Reject Credit Evaluation
```http
PUT /api/CreditEvaluation/reject
Content-Type: application/json

{
  "applicationId": 1,
  "userId": 2,
  "comments": "High DTI ratio"
}
```

### Approval API

#### Approve Loan
```http
POST /api/Approval/approve
Content-Type: application/json

{
  "applicationId": 1,
  "approverUserId": 3,
  "approvedAmount": 450000,
  "tenureMonths": 180,
  "interestRate": 8.5,
  "comments": "Approved with conditions"
}
```

#### Reject Loan
```http
POST /api/Approval/reject
Content-Type: application/json

{
  "applicationId": 1,
  "approverUserId": 3,
  "reason": "Insufficient income",
  "comments": "Income below threshold"
}
```

#### Get Approval by Application
```http
GET /api/Approval/application/{applicationId}
```

#### Get Approval History
```http
GET /api/Approval/history
```

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": ["Email is required", "Password must be at least 6 characters"]
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Invalid credentials"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "An error occurred while processing your request"
}
```

## Status Codes
- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Pagination
Search endpoints support pagination using the following response format:

```json
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 100,
    "page": 1,
    "pageSize": 10
  }
}
```

## Future Enhancements
- JWT token authentication
- API versioning
- Rate limiting
- API documentation with Swagger/OpenAPI
- Request validation with FluentValidation
- Response caching
- Bulk operations
