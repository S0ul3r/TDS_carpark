# Car Park Management API

## Project Structure

```
TDS_carpark/
├── README.md                    # Documentation
├── task_requirements.md         # Task requirements
├── setup-db.sh                  # Database setup script
├── CarParkApi.sln               # Solution file
├── CarParkApi/                  # Main API project
│   ├── appsettings.json         # Configuration
│   ├── CarParkApi.csproj        # Project file
│   ├── Program.cs               # Entry point
│   ├── Controllers/             # API endpoints
│   ├── Services/                # Business logic
│   ├── Data/                    # Database layer
│   │   ├── CarParkDbContext.cs
│   │   ├── CarParkRepository.cs
│   │   ├── ICarParkRepository.cs
│   │   └── Migrations/          # EF Core migrations
│   ├── Models/                  # DTOs and entities
│   └── Enums/                   # VehicleType enum
└── CarParkApi.Tests/            # Unit tests
    ├── Controllers/
    └── Services/
```

## Quick Setup

### 1. Install Prerequisites

**Install .NET 9.0 SDK:**
- Download: https://dotnet.microsoft.com/download/dotnet/9.0
- Verify: `dotnet --version`

**Install PostgreSQL:**
- macOS: `brew install postgresql@16 && brew services start postgresql@16`
- Or download installer from postgresql.org
- Verify: `psql --version`

**Configure PATH (macOS):**
Add to `~/.zshrc`:
```bash
export PATH="/usr/local/share/dotnet:$PATH"
export PATH="/Library/PostgreSQL/18/bin:$PATH"  # Or your PostgreSQL version
export PATH="$PATH:$HOME/.dotnet/tools"
```
Then run: `source ~/.zshrc`

### 2. Clone/Copy Project

```bash
cd ~/Documents/Projects
# Copy TDS_carpark folder or clone from git
```

### 3. Configure Database Connection

Edit `CarParkApi/appsettings.json` and update the username:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=carpark_db;Username=YOUR_USERNAME"
}
```

Replace `YOUR_USERNAME` with your macOS username (e.g., `konik`).

### 4. Setup Database

```bash
cd TDS_carpark
chmod +x setup-db.sh
./setup-db.sh
```

This creates the `carpark_db` database. EF Core migrations will create tables automatically.

### 5. Run the Application

```bash
cd CarParkApi
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

**Access:**
- Swagger UI: http://localhost:5267/swagger
- API Base: http://localhost:5267

The app automatically:
- Applies EF Core migrations
- Creates `parking_spaces` table
- Seeds 20 empty parking spaces

### 5. Run Tests

```bash
cd ..  # Back to solution root
dotnet test
```

### 6. Stop the Application

Press `Ctrl+C` in terminal, or:
```bash
lsof -ti:5267 | xargs kill -9
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/parking` | Park a vehicle (returns space number) |
| GET | `/parking` | Get available/occupied space counts |
| POST | `/parking/exit` | Process vehicle exit (returns charge) |

### Example Requests

**1. Park a vehicle:**
```bash
curl -X POST http://localhost:5267/parking \
  -H "Content-Type: application/json" \
  -d '{"vehicleReg":"ABC123","vehicleType":"Medium"}'
```

Response:
```json
{
  "vehicleReg": "ABC123",
  "spaceNumber": 1,
  "timeIn": "2025-10-06T08:53:24.592Z"
}
```

**2. Check space status:**
```bash
curl http://localhost:5267/parking
```

Response:
```json
{
  "availableSpaces": 19,
  "occupiedSpaces": 1
}
```

**3. Process vehicle exit:**
```bash
curl -X POST http://localhost:5267/parking/exit \
  -H "Content-Type: application/json" \
  -d '{"vehicleReg":"ABC123"}'
```

Response:
```json
{
  "vehicleReg": "ABC123",
  "vehicleCharge": 2.20,
  "timeIn": "2025-10-06T08:53:24.592Z",
  "timeOut": "2025-10-06T09:03:24.592Z"
}
```

## Charging Logic

**Formula:** `(Rate/min × Minutes) + (Floor(Minutes ÷ 5) × £1)`

**Rates:**
- Small Car: £0.10/minute
- Medium Car: £0.20/minute  
- Large Car: £0.40/minute
- Additional: £1.00 every 5 minutes

## Assumptions

1. **Fixed capacity:** Car park has exactly 20 spaces
2. **Space allocation:** First available space (lowest number) is assigned
3. **Vehicle registration:** Must be unique, case-sensitive, max 20 characters
4. **Vehicle types:** Only `Small`, `Medium`, or `Large` (case-insensitive input)
5. **Time calculation:** Partial minutes rounded up (ceiling)
6. **No history:** Vehicle data cleared on exit
7. **Duplicate prevention:** Vehicle already parked cannot park again
8. **UTC timestamps:** All times stored and returned in UTC
9. **Charge rounding:** Final charge rounded to 2 decimal places

## Architecture

Clean architecture with three layers:
- **Controllers** - API endpoints and request validation
- **Services** - Business logic and charge calculation
- **Repository** - Data access via EF Core

## Database Schema

**Table:** `parking_spaces`
- `id` - Primary key
- `vehicle_reg` - Unique registration (nullable)
- `vehicle_type` - Small/Medium/Large (nullable)
- `space_number` - Physical space number (1-20, unique)
- `time_in` - Entry timestamp (nullable)
- `time_out` - Exit timestamp (nullable)
- `is_occupied` - Boolean flag

## Questions

1. Should we keep historical parking records for reporting/auditing?
2. Is there a maximum parking duration or charge cap?
3. Do we need authentication/authorization for the API?
4. What happens if payment fails? Should we still release the space?