output "sql_bapi_name" {
    description = "The primary DB name for bapi"
    value = azurerm_sql_database.sql_bapi.name
    sensitive   = false
}


output "sql_isapi_name" {
    description = "The primary DB name for isapi"
    value = azurerm_sql_database.sql_isapi.name
    sensitive   = false
}

output "sql_ordapi_name" {
    description = "The primary DB name for ordapi"
    value = azurerm_sql_database.sql_ordapi.name
    sensitive   = false
}
