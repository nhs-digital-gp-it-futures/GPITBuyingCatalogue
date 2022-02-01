resource "azurerm_mssql_database" "sql_main_primary_replica" {
  name                = "BuyingCatalogue${var.db_name}${var.environment}"
  count               = var.enable_replica
  create_mode         = "OnlineSecondary"
  server_id           = data.azurerm_sql_server.sql_replica_server[0].id
  creation_source_database_id  = azurerm_mssql_database.sql_main_primary[0].id
  sku_name                     = var.core_env != "dev" ? "S1" : "S0"
  tags = {
    environment                    = var.environment,
    architecture                   = "new"
  }

    lifecycle {
    ignore_changes = [
      server_id
    ]
  }
}
