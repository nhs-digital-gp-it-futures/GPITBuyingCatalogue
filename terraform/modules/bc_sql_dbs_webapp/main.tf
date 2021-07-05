resource "azurerm_sql_database" "sql_main" {
  name                             = "BuyingCatalogue${var.db_name}${var.environment}"
  resource_group_name              = var.rg_name 
  location                         = var.region
  server_name                      = var.sqlsvr_name 
  collation                        = var.sql_collation
  edition                          = var.sql_edition
  requested_service_objective_name = var.sql_size

  tags = {
    environment                    = var.environment,
    architecture                   = "new"
  }

  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}

resource "azurerm_sql_database" "sql_user" {
  name                             = "CatalogueUser${var.db_name}${var.environment}"
  resource_group_name              = var.rg_name 
  location                         = var.region
  server_name                      = var.sqlsvr_name
  collation                        = var.sql_collation
  edition                          = var.sql_edition
  requested_service_objective_name = var.sql_size

  tags = {
    environment                    = var.environment,
    architecture                   = "new"
  }

  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}

resource "azurerm_sql_database" "sql_ordering" {
  name                             = "CatalogueOrdering${var.db_name}${var.environment}"
  resource_group_name              = var.rg_name 
  location                         = var.region
  server_name                      = var.sqlsvr_name
  collation                        = var.sql_collation
  edition                          = var.sql_edition
  requested_service_objective_name = var.sql_size

  tags = {
    environment                    = var.environment,
    architecture                   = "new"
  }

  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}
