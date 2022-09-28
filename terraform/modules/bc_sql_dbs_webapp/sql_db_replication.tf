data "azurerm_mssql_server" "sql_replica_server" {
  name                = var.sqlsvr_replica_name
  resource_group_name = var.rg_replica_name
  count               = var.enable_replica
}

resource "azurerm_mssql_failover_group" "sql_bapi_primary_fog" {
  name                = "${var.project}-${var.environment}-sql-fog-bapi-primary"
  count               = var.enable_replica
  server_id           = var.server_id
  databases           = [azurerm_mssql_database.sql_main_primary.id]
  
  partner_server {
    id = data.azurerm_mssql_server.sql_replica_server[0].id
  }
  
  read_write_endpoint_failover_policy {
    mode          = "Automatic"
    grace_minutes = 60
  }

  depends_on = [
    azurerm_mssql_database.sql_main_primary,
    azurerm_mssql_database.sql_main_primary_replica
  ]
}
