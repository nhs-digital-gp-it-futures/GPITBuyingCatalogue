resource "azurerm_sql_database" "sql_main_primary_replica" {
  name                = "BuyingCatalogue${var.db_name}${var.environment}"
  count               = var.enable_replica
  resource_group_name = var.rg_replica_name
  location            = var.region_replica
  server_name         = var.sqlsvr_replica_name
  create_mode         = "OnlineSecondary"
  source_database_id  = azurerm_sql_database.sql_main_primary.id
  tags = {
    environment                    = var.environment,
    architecture                   = "new"
  }
}
