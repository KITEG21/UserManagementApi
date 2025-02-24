# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the NuGet configuration file
COPY NuGet.config ./

# Copy the project files and restore dependencies
COPY *.sln .
COPY FullWebApi.Api/*.csproj ./FullWebApi.Api/
COPY FullWebApi.Application/*.csproj ./FullWebApi.Application/
COPY FullWebApi.Domain/*.csproj ./FullWebApi.Domain/
COPY FullWebApi.Infrastructure/*.csproj ./FullWebApi.Infrastructure/
COPY FullWebApi.Test/*.csproj ./FullWebApi.Test/
RUN dotnet restore --disable-parallel --no-cache --verbosity detailed || \
    (sleep 5 && dotnet restore --disable-parallel --no-cache --verbosity detailed) || \
    (sleep 10 && dotnet restore --disable-parallel --no-cache --verbosity detailed)

# Copy the remaining files and build the application
COPY FullWebApi.Api/. ./FullWebApi.Api/
COPY FullWebApi.Application/. ./FullWebApi.Application/
COPY FullWebApi.Domain/. ./FullWebApi.Domain/
COPY FullWebApi.Infrastructure/. ./FullWebApi.Infrastructure/
COPY FullWebApi.Test/. ./FullWebApi.Test/
WORKDIR /app/FullWebApi.Api
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/FullWebApi.Api/out ./

EXPOSE 80 


ENTRYPOINT ["dotnet", "FullWebApi.Api.dll"]