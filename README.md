# LOCPS — Loan Origination & Credit Processing System

A premium, fully-responsive **ASP.NET Core MVC (.NET 8)** frontend showcase implementing a complete banking loan origination workflow. This project is a **UI + Navigation + Controller-shell** build — no database, no business logic in C#, no APIs, no real authentication.

All dynamic behavior (charts, filtering, sorting, pagination, wizards, EMI calculations, drag-and-drop, etc.) is implemented in **client-side vanilla JavaScript**.

---

## Quick Start

```bash
cd LOCPS
dotnet build       # Verify the build compiles cleanly
dotnet run         # Launch on https://localhost:5001 (or http://localhost:5000)
```

Then open your browser and navigate to `https://localhost:5001`.

---

## Project Structure

This project now uses a role-based folder layout under `Roles/`.
The old root `Controllers/` and `Views/` folders were consolidated into role-specific locations.

```
LOCPS/
├── Roles/
│   ├── Admin/
│   │   ├── Controllers/
│   │   │   ├── ProductController.cs
│   │   │   ├── ReportsController.cs
│   │   │   └── UserManagementController.cs
│   │   └── Views/
│   │       ├── Product/
│   │       ├── Reports/
│   │       ├── UserManagement/
│   │       └── Settings/
│   ├── Customer/
│   │   ├── Controllers/
│   │   │   └── CustomerController.cs
│   │   └── Views/
│   │       ├── Customer/
│   │       ├── Dashboard/
│   │       ├── Loan/
│   │       ├── Notification/
│   │       └── Settings/
│   ├── LoanOfficer/
│   │   ├── Controllers/
│   │   │   ├── CreditController.cs
│   │   │   ├── DocumentController.cs
│   │   │   ├── KycController.cs
│   │   │   └── LoanController.cs
│   │   └── Views/
│   │       ├── Credit/
│   │       ├── Document/
│   │       ├── Kyc/
│   │       ├── Loan/
│   │       ├── Dashboard/
│   │       ├── Notification/
│   │       └── Settings/
│   ├── Underwriter/
│   │   ├── Controllers/
│   │   │   ├── ApprovalController.cs
│   │   │   └── DisbursementController.cs
│   │   └── Views/
│   │       ├── Approval/
│   │       ├── Disbursement/
│   │       └── Dashboard/
│   └── Shared/
│       ├── Controllers/
│       │   ├── AccountController.cs
│       │   ├── DashboardController.cs
│       │   ├── HomeController.cs
│       │   ├── NotificationController.cs
│       │   └── SettingsController.cs
│       └── Views/
│           ├── Account/
│           ├── Home/
│           ├── Shared/
│           ├── Dashboard/
│           ├── Document/
│           ├── Notification/
│           ├── Disbursement/
│           └── Settings/
├── wwwroot/
│   ├── css/theme.css              # Custom design system (documented palette)
│   └── js/
│       ├── sidebar.js             # Role switcher + collapsible sidebar
│       ├── toast.js               # Toast notification engine
│       ├── modal.js               # Confirmation modal controller
│       ├── data-grid.js           # Client-side search/filter/sort/paginate
│       ├── dashboard.js           # Dashboard KPI + activity init
│       ├── charts.js              # Chart.js theme defaults & helpers
│       ├── loan-wizard.js         # Multi-step application wizard
│       ├── emi-calculator.js      # EMI formula computation
│       ├── kyc.js                 # KYC verification simulation
│       ├── credit-evaluation.js   # Credit gauge animation & rules
│       └── document-upload.js     # Drag-and-drop upload with progress
└── Program.cs                     # MVC route registration only
```

---

## Design System (theme.css)

| Token               | Value       | Usage                           |
|----------------------|-------------|---------------------------------|
| `--primary-blue`     | `#1A56DB`   | Buttons, links, active states   |
| `--secondary-navy`   | `#0B2447`   | Sidebar background              |
| `--body-bg`          | `#F4F6F9`   | Page background                 |
| `--surface-white`    | `#FFFFFF`   | Cards, panels                   |
| `--success`          | `#10B981`   | Approved, verified badges       |
| `--warning`          | `#F59E0B`   | Pending, review badges          |
| `--danger`           | `#EF4444`   | Rejected, failed badges         |
| `--info`             | `#06B6D4`   | Processing, active badges       |

**Font**: Inter (Google Fonts CDN)
**Icons**: Font Awesome 6.4 (CDN)
**Charts**: Chart.js (CDN)

---

## Roles (Demo Mode)

Use the **role switcher dropdown** in the sidebar footer to toggle between:

| Role            | Accessible Sections                                                    |
|-----------------|------------------------------------------------------------------------|
| **Loan Officer**| Customers, Loans, KYC, Credit, Documents, Dashboard, Notifications     |
| **Underwriter** | Approval Review (Approve/Reject), Dashboard, Notifications             |
| **Admin**       | Products, Users, Settings, Scoring Rules, Reports, Disbursements       |

The role is stored in `localStorage` and persists across page navigations.

---

## Workflow Stepper

Every transactional page displays the shared stepper component showing progress through:

> Dashboard → Customer Registration → Loan Application → KYC Verification → Credit Evaluation → Document Upload → Document Validation → Underwriter Review → Approve/Reject → Disbursement → Completed

Set `ViewData["CurrentStep"]` in any view to highlight the active step.

---

## Where to Plug In Real Backend Logic

This project is designed as a **frontend architecture showcase**. To integrate a real backend:

1. **Add Entity Framework Core** — define `DbContext`, models, and migrations.
2. **Create Service Layer** — inject services into controllers via DI.
3. **Replace `return View()`** — fetch data from services and pass ViewModels.
4. **Replace static JS datasets** — call Web API endpoints from JS `fetch()`.
5. **Add Identity** — replace the demo login with ASP.NET Core Identity.
6. **Add `[Authorize]` attributes** — protect routes with real role checks.

---

## License

Internal demonstration project. Not licensed for production use.
