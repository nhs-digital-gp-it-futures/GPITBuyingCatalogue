FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS dacpacbuild
WORKDIR /dacpac
COPY database/NHSD.BuyingCatalogue.Identity.UserDatabase.Deployment/*.csproj .
COPY database/NHSD.BuyingCatalogue.Identity.UserDatabase .
RUN rm -rf Deployment \
    && dotnet build "NHSD.BuyingCatalogue.Identity.UserDatabase.Deployment.csproj" -c Release -o build

FROM mcr.microsoft.com/mssql-tools:latest AS dacfx
RUN apt-get update && apt-get install libunwind8 libicu-dev wget unzip -y
WORKDIR /deploy-db
RUN wget https://go.microsoft.com/fwlink/?linkid=2128144 -O sqlpackage.zip \
    && mkdir sqlpackage \
    && unzip sqlpackage.zip -d /sqlpackage \
    && chmod a+x /sqlpackage/sqlpackage
RUN apt update && apt install iputils-ping -y
COPY --from=dacpacbuild /dacpac/build/*.dacpac .
COPY database/NHSD.BuyingCatalogue.Identity.UserDatabase.Deployment/entrypoint.sh .
COPY database/NHSD.BuyingCatalogue.Identity.UserDatabase/Deployment .
RUN ["chmod", "+x", "./entrypoint.sh"]
ENTRYPOINT ["/bin/bash", "entrypoint.sh"]
