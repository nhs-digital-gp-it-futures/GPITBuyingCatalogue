output "sql_server_name" {
    description = "The new SQL server name"
    value = join("", azurerm_mssql_server.sql_server[*].name)
    sensitive   = false
}

output "sql_server_id" {
    description = "The new SQL server id"
    value = join("", azurerm_mssql_server.sql_server[*].id)
    sensitive   = false
}
