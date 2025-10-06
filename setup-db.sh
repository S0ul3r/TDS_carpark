# Check if PostgreSQL is installed
if ! command -v psql &> /dev/null; then
    echo "Error: PostgreSQL is not installed"
    echo "Install with: brew install postgresql@16"
    exit 1
fi

# Check if PostgreSQL is running
if ! pg_isready -h localhost -p 5432 &> /dev/null; then
    echo "Error: PostgreSQL is not running"
    echo "Start with: brew services start postgresql@16"
    exit 1
fi

# Check if database already exists
if psql -d postgres -lqt 2>/dev/null | cut -d \| -f 1 | grep -qw carpark_db; then
    echo "Database 'carpark_db' already exists"
    read -p "Recreate database? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        psql -d postgres -c "DROP DATABASE carpark_db;" 2>/dev/null
        psql -d postgres -c "CREATE DATABASE carpark_db;" 2>/dev/null
        echo "Database recreated"
    else
        echo "Using existing database"
    fi
else
    psql -d postgres -c "CREATE DATABASE carpark_db;" 2>/dev/null
    if [ $? -eq 0 ]; then
        echo "Database 'carpark_db' created"
    else
        echo "Error: Failed to create database"
        exit 1
    fi
fi

echo "Setup complete"
