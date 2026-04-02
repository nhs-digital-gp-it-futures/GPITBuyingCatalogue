FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS dacpacbuild
WORKDIR /dacpac
COPY database/NHSD.GPITBuyingCatalogue.Database .
RUN dotnet build "NHSD.GPITBuyingCatalogue.Database.sqlproj" -c Release -o build

FROM mcr.microsoft.com/mssql-tools:latest AS dacfx
RUN apt-get update && apt-get install libunwind8 libicu-dev wget unzip -y
WORKDIR /deploy-db
RUN wget https://go.microsoft.com/fwlink/?linkid=2338525 -O sqlpackage.zip \
    && mkdir sqlpackage \
    && unzip sqlpackage.zip -d /sqlpackage \
    && chmod a+x /sqlpackage/sqlpackage
COPY --from=dacpacbuild /dacpac/build/*.dacpac .
COPY database/NHSD.GPITBuyingCatalogue.Database/entrypoint.sh .

RUN chmod a+x entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]
