output "sql_main_dbname" {
    description = "The primary DB name for main db"
    value = azurerm_sql_database.sql_main.name
    sensitive   = false
}

output "sql_user_dbname" {
    description = "The primary DB name for user db"
    value = azurerm_sql_database.sql_user.name
    sensitive   = false
}

output "sql_ordering_dbname" {
    description = "The primary DB name for ordering db"
    value = azurerm_sql_database.sql_ordering.name
    sensitive   = false
}
