# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files individually to leverage Docker caching
COPY HackerNewsReader.Api/HackerNewsReader.Api.csproj HackerNewsReader.Api/
COPY HackerNewsReader.Application/HackerNewsReader.Application.csproj HackerNewsReader.Application/
COPY HackerNewsReader.Domain/HackerNewsReader.Domain.csproj HackerNewsReader.Domain/
COPY HackerNewsReader.Infrastructure/HackerNewsReader.Infrastructure.csproj HackerNewsReader.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the application
RUN dotnet publish HackerNewsReader.Api/HackerNewsReader.Api.csproj -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from the build stage
COPY --from=build /app/publish .

# Expose port 80 for the application
EXPOSE 80

# Define the entry point for the container
ENTRYPOINT ["dotnet", "HackerNewsReader.Api.dll"]
