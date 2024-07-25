FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NHSD.GPIT.BuyingCatalogue.Identity/NHSD.GPIT.BuyingCatalogue.Identity.csproj", "src/NHSD.GPIT.BuyingCatalogue.Identity/"]
COPY ["src/NHSD.GPIT.BuyingCatalogue.EntityFramework/NHSD.GPIT.BuyingCatalogue.EntityFramework.csproj", "src/NHSD.GPIT.BuyingCatalogue.EntityFramework/"]
RUN dotnet restore "src/NHSD.GPIT.BuyingCatalogue.Identity/NHSD.GPIT.BuyingCatalogue.Identity.csproj"
COPY . .
WORKDIR "/src/src/NHSD.GPIT.BuyingCatalogue.Identity"
RUN dotnet build "NHSD.GPIT.BuyingCatalogue.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NHSD.GPIT.BuyingCatalogue.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "NHSD.GPIT.BuyingCatalogue.Identity.dll"]
