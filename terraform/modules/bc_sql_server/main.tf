resource "azurerm_sql_server" "sql_server" {
  name                         = var.sqlsvr_name
  resource_group_name          = var.rg_name
  location                     = var.region
  version                      = var.sql_version
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  count                        = var.environment != "dr" ? 1 : 0
  tags = {
    environment                = var.environment,
    architecture               = "new"
  }
}
