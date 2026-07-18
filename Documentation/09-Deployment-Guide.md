# LOCPS - Deployment Guide

## Prerequisites

### Development Environment
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- SQL Server 2019+ or SQL Server Express
- Git

### Production Environment
- Windows Server 2019+ or Linux (Ubuntu 20.04+)
- .NET 8.0 Runtime
- SQL Server 2019+
- IIS 10.x (Windows) or Nginx/Apache (Linux)
- SSL Certificate

## Database Setup

### Local Development

1. **Create Database**
   ```sql
   CREATE DATABASE LOCPS_Dev;
   ```

2. **Configure Connection String**
   Update `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "MyConn": "Server=localhost;Database=LOCPS_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Apply Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Seed Data**
   Seed data is automatically applied via `DbInitializer.InitializeAsync()` on startup.

### Production Database

1. **Create Database**
   ```sql
   CREATE DATABASE LOCPS_Prod;
   ```

2. **Configure Connection String**
   Update `appsettings.Production.json`:
   ```json
   {
     "ConnectionStrings": {
       "MyConn": "Server=prod-server;Database=LOCPS_Prod;User Id=app_user;Password=secure_password;TrustServerCertificate=False;"
     }
   }
   ```

3. **Apply Migrations**
   ```bash
   dotnet ef database update --connection "Server=prod-server;Database=LOCPS_Prod;User Id=app_user;Password=secure_password;"
   ```

4. **Create SQL User**
   ```sql
   CREATE USER app_user WITH PASSWORD 'secure_password';
   ALTER ROLE db_datareader ADD MEMBER app_user;
   ALTER ROLE db_datawriter ADD MEMBER app_user;
   ```

## Build Configuration

### Development Build
```bash
dotnet build --configuration Debug
```

### Release Build
```bash
dotnet build --configuration Release
```

### Publish
```bash
dotnet publish -c Release -o ./publish
```

## Deployment Options

### Option 1: IIS (Windows)

#### Prerequisites
- Install .NET 8.0 Hosting Bundle
- Install IIS
- Enable ASP.NET Core modules

#### Steps

1. **Publish Application**
   ```bash
   dotnet publish -c Release -o C:\inetpub\wwwroot\LOCPS
   ```

2. **Configure IIS**
   - Create new site in IIS Manager
   - Point to published folder
   - Configure application pool (No Managed Code)
   - Enable SSL

3. **Web.config**
   Ensure `web.config` is generated in publish folder:
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <system.webServer>
       <handlers>
         <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
       </handlers>
       <aspNetCore processPath="dotnet" arguments=".\LOCPS.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" />
     </system.webServer>
   </configuration>
   ```

4. **Permissions**
   - Grant IIS AppPool identity read/write access to folder
   - Grant database access

### Option 2: Docker

#### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LOCPS.csproj", "./"]
RUN dotnet restore "LOCPS.csproj"
COPY . .
RUN dotnet build "LOCPS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LOCPS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LOCPS.dll"]
```

#### Build and Run
```bash
# Build image
docker build -t locps:latest .

# Run container
docker run -d -p 8080:80 --name locps -e ConnectionStrings__MyConn="Server=db;Database=LOCPS;User Id=sa;Password=YourPassword123" locps:latest
```

#### Docker Compose
```yaml
version: '3.8'
services:
  locps:
    build: .
    ports:
      - "8080:80"
    environment:
      - ConnectionStrings__MyConn=Server=sqlserver;Database=LOCPS;User Id=sa;Password=YourPassword123
    depends_on:
      - sqlserver
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
    ports:
      - "1433:1433"
```

### Option 3: Linux with Nginx

#### Prerequisites
- Install .NET 8.0 Runtime
- Install Nginx
- Configure as systemd service

#### Steps

1. **Publish Application**
   ```bash
   dotnet publish -c Release -o /var/www/locps
   ```

2. **Create Systemd Service**
   ```ini
   [Unit]
   Description=LOCPS Application
   After=network.target

   [Service]
   Type=notify
   WorkingDirectory=/var/www/locps
   ExecStart=/usr/bin/dotnet /var/www/locps/LOCPS.dll
   Restart=always
   RestartSec=10
   SyslogIdentifier=locps
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production
   Environment=ConnectionStrings__MyConn="Server=localhost;Database=LOCPS;User Id=locps_user;Password=secure_password"

   [Install]
   WantedBy=multi-user.target
   ```

3. **Enable Service**
   ```bash
   sudo systemctl enable locps
   sudo systemctl start locps
   ```

4. **Configure Nginx**
   ```nginx
   server {
       listen 80;
       server_name locps.example.com;
       return 301 https://$server_name$request_uri;
   }

   server {
       listen 443 ssl http2;
       server_name locps.example.com;

       ssl_certificate /etc/ssl/certs/locps.crt;
       ssl_certificate_key /etc/ssl/private/locps.key;

       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header X-Forwarded-Proto $scheme;
       }
   }
   ```

## Configuration

### Environment Variables
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__MyConn="Server=prod-server;Database=LOCPS;User Id=app_user;Password=secure_password"
```

### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "MyConn": "Server=prod-server;Database=LOCPS;User Id=app_user;Password=secure_password;TrustServerCertificate=False;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

## Security Hardening

### SSL/TLS
- Enable HTTPS only in production
- Use strong TLS protocols (1.2, 1.3)
- Implement HSTS
- Use valid SSL certificates

### Cookie Security
```csharp
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
options.Cookie.HttpOnly = true;
options.Cookie.SameSite = SameSiteMode.Strict;
```

### Database Security
- Use strong passwords
- Limit database user permissions
- Enable encryption at rest
- Regular backups

### Application Security
- Remove debug information
- Disable detailed error pages
- Implement rate limiting
- Enable request size limits
- Add security headers

## Monitoring

### Logging
- Configure structured logging (Serilog)
- Log to file and external service
- Monitor error rates
- Set up alerts

### Performance
- Monitor response times
- Track database query performance
- Monitor memory usage
- Set up performance counters

### Health Checks
Add health check endpoint:
```csharp
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
```

## Backup Strategy

### Database Backup
```sql
BACKUP DATABASE LOCPS_Prod 
TO DISK = 'C:\Backups\LOCPS_Prod.bak'
WITH FORMAT, COMPRESSION;
```

### Application Backup
- Backup published files
- Version control deployment
- Rollback procedure

## Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check connection string
   - Verify SQL Server is running
   - Check firewall rules
   - Verify user permissions

2. **Migration Errors**
   - Ensure database exists
   - Check user has schema modification rights
   - Verify migration history

3. **Application Won't Start**
   - Check .NET runtime installation
   - Verify configuration files
   - Check application logs
   - Verify file permissions

4. **Performance Issues**
   - Check database indexing
   - Monitor query performance
   - Review caching strategy
   - Check server resources

## Rollback Procedure

1. **Database Rollback**
   ```bash
   dotnet ef database update <previous-migration>
   ```

2. **Application Rollback**
   - Restore previous published files
   - Restart application service
   - Verify functionality

## Maintenance

### Regular Tasks
- Review and apply security patches
- Update .NET runtime
- Optimize database indexes
- Review and archive logs
- Monitor disk space
- Test backup restoration

### Update Procedure
1. Test in staging environment
2. Create database backup
3. Apply migrations
4. Deploy application
5. Verify functionality
6. Monitor for issues
