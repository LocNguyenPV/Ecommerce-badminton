# See https://aka.ms/customizecontainer to learn how to customize your debug container 
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ECommerce.API/ECommerce.API.csproj", "ECommerce.API/"]
COPY ["ECommerce.Domain/ECommerce.Domain.csproj", "ECommerce.Domain/"]
COPY ["ECommerce.Application/ECommerce.Application.csproj", "ECommerce.Application/"]
COPY ["ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "ECommerce.Infrastructure/"]
RUN dotnet restore "./ECommerce.API/ECommerce.API.csproj"
COPY . .
WORKDIR "/src/ECommerce.API"
RUN dotnet build "./ECommerce.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Install dotnet-ef tool
RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet publish "./ECommerce.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Create migration bundle
WORKDIR /src
RUN dotnet ef migrations bundle \
    --project ECommerce.Infrastructure \
    --startup-project ECommerce.API \
    --output /app/publish/efbundle \
    --self-contained

# Stage 4: Final runtime image
FROM base AS final

# Switch to root before installing packages
USER root
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && \
    apt-get install -y curl libicu-dev && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Switch back to app user
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
