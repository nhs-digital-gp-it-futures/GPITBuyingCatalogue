resource "azurerm_mssql_database" "sql_main_primary" {
  name                             = "BuyingCatalogue${var.db_name}${var.environment}"  
  count                            = var.environment != "dr" ? 1 : 0
  server_id                        = var.server_id  
  collation                        = var.sql_collation  
  sku_name                         = var.core_env != "dev" ? "S1" : "S0"
 
  tags = {
    environment                    = var.environment,
    architecture                   = "new"
  }

  short_term_retention_policy {
    retention_days = var.core_env != "dev" ? 30 : 7
  }

  long_term_retention_policy {
    weekly_retention = var.core_env != "dev" ? "P12W" : null
    monthly_retention = var.core_env != "dev" ? "P12M" : null
    yearly_retention = var.core_env != "dev" ? "P6Y" : null
    week_of_year = 1
  }
  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}
