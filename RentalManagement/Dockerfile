# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the csproj and restore as distinct layers
COPY RentalManagement/*.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY RentalManagement/. ./

# Publish the application
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Run your app
ENTRYPOINT ["dotnet", "RentalManagement.dll"]
