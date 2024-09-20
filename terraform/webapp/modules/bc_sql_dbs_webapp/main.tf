resource "azurerm_mssql_database" "sql_main_primary" {
  name      = "BuyingCatalogue-${var.environment}"
  server_id = var.server_id
  collation = var.sql_collation
  sku_name  = var.core_env != "dev" ? "S1" : "S0"

  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  short_term_retention_policy {
    retention_days = var.core_env != "dev" ? 30 : 7
  }

  long_term_retention_policy {
    weekly_retention  = var.core_env != "dev" ? "P12W" : null
    monthly_retention = var.core_env != "dev" ? "P12M" : null
    yearly_retention  = var.core_env != "dev" ? "P6Y" : null
    week_of_year      = 1
  }
  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}

data "azurerm_mssql_server" "sql_replica_server" {
  name                = var.sqlsvr_replica_name
  resource_group_name = var.rg_replica_name
  count               = var.enable_replica
}

resource "azurerm_mssql_failover_group" "sql_bapi_primary_fog" {
  name      = "${var.project}-${var.environment}-sql-fog-bapi-primary"
  count     = var.enable_replica
  server_id = var.server_id
  databases = [azurerm_mssql_database.sql_main_primary.id]

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

resource "azurerm_mssql_database" "sql_main_primary_replica" {
  name                        = "BuyingCatalogue-${var.environment}"
  count                       = var.enable_replica
  create_mode                 = "Secondary"
  server_id                   = data.azurerm_mssql_server.sql_replica_server[0].id
  creation_source_database_id = azurerm_mssql_database.sql_main_primary.id
  sku_name                    = var.core_env != "dev" ? "S1" : "S0"
  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  lifecycle {
    ignore_changes = [
      server_id
    ]
  }
}
