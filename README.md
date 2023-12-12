# GPITBuyingCatalogue
ASP.NET Core Buying Catalogue Web Application 

This repository contains a revised architecture for the Buying Catalogue.

Once complete, this will become the single repository required for the Buying Catalogue.

## IMPORTANT NOTES

**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Architecture overview

This application provides a Web Application capable of running on Linux or Windows.


## Setting up your development environment for the Buying Catalogue

### Requirements

- [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [NodeJS (v17.9.1)](https://nodejs.org/dist/latest-v17.x/node-v17.9.1-x64.msi)
- [Rancher Desktop](https://rancherdesktop.io/)

**Note**: Docker Desktop can also be used if you'd prefer. However, Docker Desktop frequently has installation or runtime issues that will go un-patched for several versions.

### NodeJS Setup

The latest version of NodeJS has an issue which prevents installation of the required dependencies.

Upon installing v17.9.1, open an elevated command prompt and install the `windows-build-tools` package via `npm install -g windows-build-tools`.

Next, `cd` to `<repository root>/src/NHSD.GPIT.BuyingCatalogue.WebApp` and from there run the following commands. 

1. `npm install`
2. `npx gulp min`

## Running the API

*All scripts are meant to be run in PowerShell from within the repository root directory.*

To setup the application prerequisites (Database, test data and Azurite), run the following scripts:

```powershell
 & '.\docker-build.ps1'
```
and then:

```powershell
 & '.\docker-start.ps1'
```

Afterwards, open the solution in Visual Studio and run/debug NHSD.GPIT.BuyingCatalogue.WebApp

## Troubleshooting

### Cannot build SQL Project in Visual Studio

The database SQL Project has been migrated to Microsoft's [Microsoft.Build.Sql](https://github.com/microsoft/DacFx/tree/main/src/Microsoft.Build.Sql) SDK to enable .NET Core support for the database project.

As a consequence, building in Visual Studio isn't currently supported. To build the database solution `dotnet build` must be used instead.

Alternatively, the preexisting `docker-compose` approach will continue to work as normal.

See [the following PR](https://github.com/nhs-digital-gp-it-futures/GPITBuyingCatalogue/pull/1134) for more information on the Microsoft.Build.Sql migration.

### SQL Server is running but there is no database

The `dacpac` deployment takes a few seconds to initialize and complete so it is not unusual for there to be a slight delay between SQL server initializing and the database being ready for use; upon completion `<DB Name> database setup complete` is logged to the console.

