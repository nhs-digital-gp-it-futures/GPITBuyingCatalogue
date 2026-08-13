#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0.5-alpine3.23 AS base
RUN apk update && apk add icu-libs
RUN apk --no-cache upgrade && apk add --no-cache chromium
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:14-alpine AS frontend
WORKDIR /src/src/NHSD.GPIT.BuyingCatalogue.WebApp
COPY ["src/NHSD.GPIT.BuyingCatalogue.WebApp/package.json", "src/NHSD.GPIT.BuyingCatalogue.WebApp/package-lock.json", "./"]
RUN npm install --ignore-scripts
COPY ["src/NHSD.GPIT.BuyingCatalogue.WebApp/", "./"]
RUN npx gulp min

FROM mcr.microsoft.com/dotnet/sdk:10.0.201-alpine3.23 AS build
WORKDIR /src
COPY ["src/NHSD.GPIT.BuyingCatalogue.WebApp/NHSD.GPIT.BuyingCatalogue.WebApp.csproj", "src/NHSD.GPIT.BuyingCatalogue.WebApp/"]
RUN dotnet restore "src/NHSD.GPIT.BuyingCatalogue.WebApp/NHSD.GPIT.BuyingCatalogue.WebApp.csproj"
COPY . .
COPY --from=frontend /src/src/NHSD.GPIT.BuyingCatalogue.WebApp/wwwroot/ /src/src/NHSD.GPIT.BuyingCatalogue.WebApp/wwwroot/
WORKDIR "/src/src/NHSD.GPIT.BuyingCatalogue.WebApp"
RUN dotnet build "NHSD.GPIT.BuyingCatalogue.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.GPIT.BuyingCatalogue.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSD.GPIT.BuyingCatalogue.WebApp.dll"]
