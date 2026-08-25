FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY AiBusinessWorkflow.sln .
COPY AiBusinessWorkflow.Api/AiBusinessWorkflow.Api.csproj AiBusinessWorkflow.Api/
COPY AiBusinessWorkflow.Tests/AiBusinessWorkflow.Tests.csproj AiBusinessWorkflow.Tests/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build
RUN dotnet publish AiBusinessWorkflow.Api/AiBusinessWorkflow.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

USER app

ENTRYPOINT ["dotnet", "AiBusinessWorkflow.Api.dll"]
