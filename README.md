# GPITBuyingCatalogue
ASP.NET Core Buying Catalogue Web Application 

This repository contains a revised architecture for the Buying Catalogue.

Once complete, this will become the single repository required for the Buying Catalogue.

## IMPORTANT NOTES

**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Architecture overview

This application uses **.NET 5** to provide a Web Application capable of running on Linux or Windows.


## Setting up your development environment for the Buying Catalogue

### Requirements

- .NET 5.0
- Docker

> Before you begin please install **.NET 5.0** & **Docker** on your machine.

## Running the API

### On a Windows Box

*All scripts are meant to be run in PowerShell from within this directory.*

To run the application in a container in development environment, run the following script:

```powershell
 & '.\docker-build.ps1'
```
and then:

```powershell
 & '.\docker-start.ps1'
```

You can now access the docker based Web App in your browser at <http://localhost:8000>.

You can also simply open the solution in Visual Studio and run/debug NHSD.GPIT.BuyingCatalogue.WebApp


## Troubleshooting

### SQL Server is running but there is no database

The `dacpac` deployment takes a few seconds to initialize and complete so it is not unusual for there to be a slight delay between SQL server initializing and the database being ready for use; upon completion `<DB Name> database setup complete` is logged to the console.

