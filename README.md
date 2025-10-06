# Car Park Management API

Simple REST API for managing a 20-space car park with automatic charge calculation.

## Prerequisites
- .NET 9.0 SDK ([download here](https://dotnet.microsoft.com/download/dotnet/9.0))
- PostgreSQL 12+ ([download here](https://www.postgresql.org/download/))

### Installation

1. **Clone the project**
   ```bash
   cd TDS_carpark
   ```

2. **Configure database**
   
   Edit `CarParkApi/appsettings.json`:
   ```json
   "DefaultConnection": "Host=localhost;Port=5432;Database=carpark_db;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
   ```

3. **Create database**
   
   Windows:
   ```cmd
   psql -U postgres -c "CREATE DATABASE carpark_db;"
   ```
   
   macOS/Linux:
   ```bash
   ./setup-db.sh
   ```

4. **Run the app**
   
   Windows:
   ```cmd
   cd CarParkApi
   set ASPNETCORE_ENVIRONMENT=Development
   dotnet run
   ```
   
   macOS/Linux:
   ```bash
   cd CarParkApi
   export ASPNETCORE_ENVIRONMENT=Development
   dotnet run
   ```

5. **Open Swagger**
   
   http://localhost:5267/swagger

## API Endpoints

- `GET /parking` - Check available spaces
- `POST /parking` - Park a vehicle
- `POST /parking/exit` - Process exit and calculate charge

### Example: Park a vehicle

```json
POST /parking
{
  "vehicleReg": "ABC123",
  "vehicleType": "Medium"
}
```

Response:
```json
{
  "vehicleReg": "ABC123",
  "spaceNumber": 1,
  "timeIn": "2025-10-06T10:30:00Z"
}
```

## Testing

### Run unit tests
```bash
dotnet test
```

## Project Structure

```
TDS_carpark/
├── CarParkApi/              # Main API
│   ├── Controllers/         # Endpoints
│   ├── Services/            # Business logic
│   ├── Data/                # Database & repo
│   ├── Models/              # DTOs
│   └── Program.cs           # Entry point
└── CarParkApi.Tests/        # Unit tests (34 tests)
```

## Assumptions Made

- **Car park capacity:** Fixed at 20 spaces (could be changed in CarParkDbContext)
- **Space allocation:** First available space assigned (lowest number first, not using maths for finding closest possible space)
- **Vehicle registration:** Unique and case-sensitive, max 20 characters
- **Vehicle types:** Only Small, Medium, or Large accepted
- **Duplicate parking:** Vehicle already parked cannot park again until it exits
- **Parking history:** No records kept after exit (could change that for keeping records)
- **Time precision:** Partial minutes rounded up using ceiling function (might use floor or some other idea)
- **Timestamps:** All stored and returned in UTC
- **Charge calculation:** Rounded to 2 decimal places
- **Concurrent access:** Thread-safe with database-level constraints
- **Payment:** Charge calculated but no payment processing (no way for now to confirm/deny payment)

## Questions I Would Ask

1. **Historical data:** Should we keep parking records after exit for auditing or reporting purposes?
2. **Maximum duration:** Is there a maximum parking duration or charge cap we should enforce?
3. **Payment integration:** Should the API handle payment processing, or just calculate charges?
4. **Authentication:** Should endpoints require authentication/authorization?
5.  **Error scenarios:** Probably missing a lot of scenarios like when vehicle exits parking but never entered etc.?
6.  **Performance:** Nice to have perfromance checks, maybe some caching implementation?