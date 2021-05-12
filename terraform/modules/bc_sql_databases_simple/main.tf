resource "azurerm_sql_database" "sql_bapi" {
  name                             = "${var.db_name}-bapi"
  resource_group_name              = var.rg_name 
  location                         = var.region
  server_name                      = var.sqlsvr_name 
  collation                        = var.sql_collation
  edition                          = var.sql_edition
  requested_service_objective_name = var.sql_size

  tags = {
    environment                    = var.environment
  }

  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}

resource "azurerm_sql_database" "sql_isapi" {
  name                             = "${var.db_name}-isapi" 
  resource_group_name              = var.rg_name 
  location                         = var.region
  server_name                      = var.sqlsvr_name
  collation                        = var.sql_collation
  edition                          = var.sql_edition
  requested_service_objective_name = var.sql_size

  tags = {
    environment                    = var.environment
  }

  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}

resource "azurerm_sql_database" "sql_ordapi" {
  name                             = "${var.db_name}-ordapi" 
  resource_group_name              = var.rg_name 
  location                         = var.region
  server_name                      = var.sqlsvr_name
  collation                        = var.sql_collation
  edition                          = var.sql_edition
  requested_service_objective_name = var.sql_size

  tags = {
    environment                    = var.environment
  }

  lifecycle {
    ignore_changes = [
      create_mode
    ]
  }
}
