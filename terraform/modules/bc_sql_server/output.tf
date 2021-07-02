output "sql_server_name" {
    description = "The new SQL server name"
    value = azurerm_sql_server.sql_server.name
    sensitive   = false
}
