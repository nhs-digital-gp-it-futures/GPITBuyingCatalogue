

resource "azurerm_resource_group" "sql-server" {
  name           = "${var.project}-${var.environment}-rg-sql-server"
  location       = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_sql_server" "sql_server" {
  name                         = var.sqlsvr_name
  resource_group_name          = azurerm_resource_group.sql-server.name
  location                     = var.region
  version                      = var.sql_version
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  tags = {
    environment                = var.environment,
    architecture               = "new"
  }
}
