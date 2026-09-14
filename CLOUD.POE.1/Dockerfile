# Build the CoffeeNChill Azure Functions project with .NET 9.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy the project file and restore NuGet packages.
COPY ["CLOUD.POE.1.csproj", "./"]

RUN dotnet restore "CLOUD.POE.1.csproj"

# Copy the remaining project files.
COPY . .

# Publish the Functions project.
RUN dotnet publish "CLOUD.POE.1.csproj" -c Release -o /app/publish

# Use the Azure Functions .NET 9 isolated runtime.
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0

# Tell the Functions runtime where the application is stored.
ENV AzureWebJobsScriptRoot=/home/site/wwwroot

# Enable console logs inside Docker.
ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true

# Copy the published application into the runtime image.
COPY --from=build /app/publish /home/site/wwwroot