#!/bin/bash
# Wait until MySQL is available
until mysql -h mysql -u root -p"qwerty" -e "select 1" &> /dev/null; do
  echo "Waiting for MySQL..."
  sleep 2
done

echo "MySQL is up. Executing schema.sql..."
# Execute the schema.sql script
mysql -h mysql -u root -p"qwerty" < /schema.sql

# Start the main application
echo "Starting CibusServer..."
exec dotnet CibusServer.dll
