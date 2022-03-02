#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
RUN apk update && apk add icu-libs
RUN apk --no-cache upgrade && apk add --no-cache chromium

#UrlValidator. See https://github.com/dotnet/SqlClient/issues/776
RUN sed -i '1i openssl_conf = default_conf' /etc/ssl/openssl.cnf && echo -e "\n[ default_conf ]\nssl_conf = ssl_sect\n[ssl_sect]\nsystem_default = system_default_sect\n[system_default_sect]\nMinProtocol = TLSv1\nCipherString = DEFAULT:@SECLEVEL=1" >> /etc/ssl/openssl.cnf

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
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
