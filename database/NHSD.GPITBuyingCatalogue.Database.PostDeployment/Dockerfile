#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["database/NHSD.GPITBuyingCatalogue.Database.PostDeployment/NHSD.GPITBuyingCatalogue.Database.PostDeployment.csproj", "database/NHSD.GPITBuyingCatalogue.Database.PostDeployment/"]
RUN dotnet restore "database/NHSD.GPITBuyingCatalogue.Database.PostDeployment/NHSD.GPITBuyingCatalogue.Database.PostDeployment.csproj"
COPY . .
WORKDIR "/src/database/NHSD.GPITBuyingCatalogue.Database.PostDeployment"
RUN dotnet build "NHSD.GPITBuyingCatalogue.Database.PostDeployment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.GPITBuyingCatalogue.Database.PostDeployment.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
SHELL ["/bin/bash", "-c"]
RUN apt-get update && apt-get install wget gnupg2 -y \
    && wget -q https://packages.microsoft.com/keys/microsoft.asc -O- | apt-key add - \
    && wget -q https://packages.microsoft.com/config/ubuntu/20.04/prod.list -O- | tee /etc/apt/sources.list.d/msprod.list \
    && apt-get update -y \
    && ACCEPT_EULA=Y apt-get install mssql-tools unixodbc-dev -y

COPY database/NHSD.GPITBuyingCatalogue.Database.PostDeployment/entrypoint.sh .
RUN chmod +x ./entrypoint.sh
COPY --from=publish /app/publish .
ENTRYPOINT ["./entrypoint.sh"]
