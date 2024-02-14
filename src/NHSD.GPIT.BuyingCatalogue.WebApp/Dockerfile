#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.1-alpine3.18 AS base
RUN apk update && apk add icu-libs
RUN apk --no-cache upgrade && apk add --no-cache chromium
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.100-alpine3.18 AS build
WORKDIR /src
COPY ["src/NHSD.GPIT.BuyingCatalogue.WebApp/NHSD.GPIT.BuyingCatalogue.WebApp.csproj", "src/NHSD.GPIT.BuyingCatalogue.WebApp/"]
RUN dotnet restore "src/NHSD.GPIT.BuyingCatalogue.WebApp/NHSD.GPIT.BuyingCatalogue.WebApp.csproj"
COPY . .
RUN apk add --update npm
WORKDIR "/src/src/NHSD.GPIT.BuyingCatalogue.WebApp"
RUN dotnet build "NHSD.GPIT.BuyingCatalogue.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.GPIT.BuyingCatalogue.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSD.GPIT.BuyingCatalogue.WebApp.dll"]
