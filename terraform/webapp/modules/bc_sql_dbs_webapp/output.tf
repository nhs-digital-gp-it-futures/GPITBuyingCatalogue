output "sql_main_dbname" {
    description = "The primary DB name for main db"
    value = join("", azurerm_mssql_database.sql_main_primary[*].name)
    sensitive   = false
}
