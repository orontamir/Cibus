FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY ["/", "CibusServer/"]
WORKDIR "/CibusServer"
RUN dotnet restore "CibusServer.csproj"
RUN dotnet build "CibusServer.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "CibusServer.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy the init script and the schema file into the container
COPY init.sh /usr/local/bin/init.sh
COPY Schema.sql /schema.sql
# Install MySQL client (using default-mysql-client on Debian-based images)
RUN apt-get update && apt-get install -y default-mysql-client && rm -rf /var/lib/apt/lists/*
RUN chmod +x /usr/local/bin/init.sh
# Use the init script as the container entrypoint
ENTRYPOINT ["/usr/local/bin/init.sh"]
