# Description

DbUp is a .NET library that helps you to deploy changes to SQL Server databases. It tracks which SQL scripts have been run already, and runs the change scripts that are needed to get your database up to date.

[Read more here](https://dbup.readthedocs.io/en/latest/)

There is a `SchemaVersions` table which will persist the script executions so DbUp can determine which scripts have already been executed and skip those in the next run.

# Script Conventions

DbUp executes scripts in a sequential order, alphabetically, based on the script name. Scripts should be named accordingly.  

The naming convention used in this project is `{5 digit script ID} - {Brief Description}`. The Script ID is a 5 digit ID that is left padded, to a max length of 5, with the number 0. The description is a brief summary of what the script is.

Example:

```
00001 - Initial Script
00002 - Cleanup a table
00003 - Migrate some data
```

The example above would result in the DbUp project executing the scripts in the order which the example is presented.
