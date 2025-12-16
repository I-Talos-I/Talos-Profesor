# Multi-stage Dockerfile for CLIProfessor.Api

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/CLIProfessor.Api/CLIProfessor.Api.csproj", "src/CLIProfessor.Api/"]
COPY ["src/CLIProfessor.Application/CLIProfessor.Application.csproj", "src/CLIProfessor.Application/"]
COPY ["src/CLIProfessor.Domain/CLIProfessor.Domain.csproj", "src/CLIProfessor.Domain/"]
COPY ["src/CLIProfessor.Infrastructure/CLIProfessor.Infrastructure.csproj", "src/CLIProfessor.Infrastructure/"]

RUN dotnet restore "src/CLIProfessor.Api/CLIProfessor.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build and Publish
WORKDIR "/src/src/CLIProfessor.Api"
RUN dotnet build "CLIProfessor.Api.csproj" -c Release -o /app/build
RUN dotnet publish "CLIProfessor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CLIProfessor.Api.dll"]
