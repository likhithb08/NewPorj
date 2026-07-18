# LOCPS - Testing Guide

## Overview
This guide covers testing strategies for the LOCPS application including unit testing, integration testing, and end-to-end testing.

## Testing Strategy

### Test Pyramid
```
        /\
       /  \      E2E Tests (few)
      /____\
     /      \    Integration Tests (moderate)
    /________\
   /          \  Unit Tests (many)
  /____________\
```

## Unit Testing

### Setup
Create test project:
```bash
dotnet new xunit -n LOCPS.Tests
dotnet add reference ../LOCPS.csproj
dotnet add package Moq
dotnet add package FluentAssertions
```

### Repository Tests

**Example: LoanApplicationRepository Tests**
```csharp
public class LoanApplicationRepositoryTests
{
    private readonly Mock<AppDbContext> _contextMock;
    private readonly LoanApplicationRepository _repository;

    public LoanApplicationRepositoryTests()
    {
        _contextMock = new Mock<AppDbContext>();
        _repository = new LoanApplicationRepository(_contextMock.Object);
    }

    [Fact]
    public async Task CreateLoanApplicationAsync_ShouldReturnCreatedApplication()
    {
        // Arrange
        var application = new LoanApplication
        {
            ApplicationNumber = "LOC-20240101-1234",
            CustomerId = 1,
            ProductId = 1,
            RequestedAmount = 500000
        };

        // Act
        var result = await _repository.CreateLoanApplicationAsync(application);

        // Assert
        result.Should().NotBeNull();
        result.ApplicationNumber.Should().Be("LOC-20240101-1234");
    }

    [Fact]
    public async Task GetByApplicationIdAsync_ShouldReturnApplication()
    {
        // Arrange
        var applicationId = 1;

        // Act
        var result = await _repository.GetLoanApplicationByIdAsync(applicationId);

        // Assert
        result.Should().NotBeNull();
        result.ApplicationId.Should().Be(applicationId);
    }
}
```

### Service Tests

**Example: LoanApplicationService Tests**
```csharp
public class LoanApplicationServiceTests
{
    private readonly Mock<ILoanApplicationRepository> _repoMock;
    private readonly Mock<ILoanProductRepository> _productRepoMock;
    private readonly Mock<IAuditLogService> _auditLogMock;
    private readonly Mock<INotificationService> _notificationMock;
    private readonly LoanApplicationService _service;

    public LoanApplicationServiceTests()
    {
        _repoMock = new Mock<ILoanApplicationRepository>();
        _productRepoMock = new Mock<ILoanProductRepository>();
        _auditLogMock = new Mock<IAuditLogService>();
        _notificationMock = new Mock<INotificationService>();
        _service = new LoanApplicationService(
            _repoMock.Object,
            _productRepoMock.Object,
            _auditLogMock.Object,
            _notificationMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateApplication()
    {
        // Arrange
        var application = new LoanApplication
        {
            CustomerId = 1,
            ProductId = 1,
            RequestedAmount = 500000,
            AnnualIncome = 600000
        };

        var product = new LoanProduct
        {
            ProductId = 1,
            MinAmount = 100000,
            MaxAmount = 10000000
        };

        _productRepoMock.Setup(x => x.GetLoanProductByIdAsync(1))
            .ReturnsAsync(product);
        _repoMock.Setup(x => x.CreateLoanApplicationAsync(It.IsAny<LoanApplication>()))
            .ReturnsAsync(application);

        // Act
        var result = await _service.CreateAsync(application);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ApplicationStatus.Submitted);
        _notificationMock.Verify(x => x.CreateAsync(
            It.IsAny<int>(),
            NotificationType.ApplicationSubmitted,
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int?>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithAmountOutsideRange_ShouldThrowException()
    {
        // Arrange
        var application = new LoanApplication
        {
            CustomerId = 1,
            ProductId = 1,
            RequestedAmount = 15000000, // Above max
            AnnualIncome = 600000
        };

        var product = new LoanProduct
        {
            ProductId = 1,
            MinAmount = 100000,
            MaxAmount = 10000000
        };

        _productRepoMock.Setup(x => x.GetLoanProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceException>(() => _service.CreateAsync(application));
    }
}
```

### Controller Tests

**Example: ProductController Tests**
```csharp
public class ProductControllerTests
{
    private readonly Mock<ILoanProductService> _serviceMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _serviceMock = new Mock<ILoanProductService>();
        _controller = new ProductController(_serviceMock.Object);
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<LoanProduct>
        {
            new LoanProduct { ProductId = 1, ProductName = "Home Loan" },
            new LoanProduct { ProductId = 2, ProductName = "Car Loan" }
        };

        _serviceMock.Setup(x => x.GetAllAsync(true))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task Create_WithValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var product = new LoanProduct
        {
            ProductName = "Personal Loan",
            MinAmount = 10000,
            MaxAmount = 500000
        };

        _serviceMock.Setup(x => x.CreateAsync(It.IsAny<LoanProduct>(), It.IsAny<int>()))
            .ReturnsAsync(product);

        // Mock User claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        // Act
        var result = await _controller.Create(product);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        ((RedirectToActionResult)result).ActionName.Should().Be("Index");
    }
}
```

## Integration Testing

### Setup
```bash
dotnet new xunit -n LOCPS.IntegrationTests
dotnet add reference ../LOCPS.csproj
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

### TestWebApplicationFactory
```csharp
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace DbContext with in-memory version
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Create database
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        });
    }
}
```

### Integration Test Example
```csharp
public class LoanApplicationIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public LoanApplicationIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateLoanApplication_ShouldCreateAndReturnApplication()
    {
        // Arrange
        var application = new CreateLoanApplicationDto
        {
            CustomerId = 1,
            ProductId = 1,
            RequestedAmount = 500000,
            AnnualIncome = 600000,
            EmploymentType = "Salaried"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(application),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/LoanApplication", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResult<LoanApplicationDto>>(responseString);
        
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.RequestedAmount.Should().Be(500000);
    }
}
```

## API Testing

### Using Postman
Create collection with endpoints:
- POST /api/User/register
- POST /api/User/login
- GET /api/LoanProduct
- POST /api/LoanApplication
- GET /api/LoanApplication/search
- PUT /api/LoanApplication/{id}/status

### Using Swagger
Add Swagger to project:
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

app.UseSwagger();
app.UseSwaggerUI();
```

## End-to-End Testing

### Using Playwright
```bash
dotnet new xunit -n LOCPS.E2ETests
dotnet add package Microsoft.Playwright
dotnet add package Microsoft.Playwright.NUnit
```

### E2E Test Example
```csharp
public class LoanApplicationE2ETests
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        _page = await _browser.NewPageAsync();
    }

    [Test]
    public async Task CompleteLoanApplicationFlow()
    {
        // Navigate to login
        await _page.GotoAsync("https://localhost:5001/Account/Login");
        
        // Login
        await _page.FillAsync("#Email", "customer@example.com");
        await _page.FillAsync("#Password", "Password123");
        await _page.ClickAsync("button[type='submit']");
        
        // Navigate to create application
        await _page.ClickAsync("text=New Application");
        
        // Fill form
        await _page.SelectOptionAsync("#ProductId", "1");
        await _page.FillAsync("#RequestedAmount", "500000");
        await _page.FillAsync("#AnnualIncome", "600000");
        await _page.SelectOptionAsync("#EmploymentType", "Salaried");
        
        // Submit
        await _page.ClickAsync("button[type='submit']");
        
        // Verify
        await Expect(_page.Locator("text=Application Submitted")).ToBeVisibleAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _browser.CloseAsync();
        await _playwright.DisposeAsync();
    }
}
```

## Test Coverage

### Running Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Viewing Coverage Report
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./coverage
```

### Coverage Goals
- Aim for >80% code coverage
- Focus on critical business logic
- Prioritize service layer coverage
- Include edge cases

## Performance Testing

### Using BenchmarkDotNet
```bash
dotnet add package BenchmarkDotNet
```

### Benchmark Example
```csharp
[MemoryDiagnoser]
public class RepositoryBenchmarks
{
    private AppDbContext _context;
    private LoanApplicationRepository _repository;

    [GlobalSetup]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("BenchmarkDb")
            .Options;
        _context = new AppDbContext(options);
        _repository = new LoanApplicationRepository(_context);
    }

    [Benchmark]
    public async Task<LoanApplication?> GetByIdAsync()
    {
        return await _repository.GetLoanApplicationByIdAsync(1);
    }

    [Benchmark]
    public async Task<IEnumerable<LoanApplication>> GetAllAsync()
    {
        return await _repository.GetAllLoanApplicationAsync();
    }
}
```

## Continuous Integration

### GitHub Actions Example
```yaml
name: CI

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Code Coverage
      run: dotnet test --collect:"XPlat Code Coverage"
```

## Best Practices

1. **Arrange-Act-Assert Pattern**: Structure tests clearly
2. **Descriptive Test Names**: Use Should_When_Then format
3. **Test Independence**: Tests should not depend on each other
4. **Mock External Dependencies**: Use mocks for services/repositories
5. **Test Edge Cases**: Include boundary conditions
6. **Keep Tests Fast**: Unit tests should run in milliseconds
7. **Use Test Data Builders**: Create test data efficiently
8. **Regular Maintenance**: Update tests with code changes
