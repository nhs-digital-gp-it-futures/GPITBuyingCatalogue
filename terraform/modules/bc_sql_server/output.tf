output "sql_server_name" {
    description = "The new SQL server name"
    value = join("", azurerm_sql_server.sql_server[*].name)
    sensitive   = false
}

output "sql_server_id" {
    description = "The new SQL server id"
    value = join("", azurerm_sql_server.sql_server[*].id)
    sensitive   = false
}

output "sql_resource_group" {
    description = "Resource group that the SQL instance belong to"
    value       = azurerm_resource_group.sql-server.name
}