data "azurerm_mssql_server" "sql_server" {
  name                = var.sqlserver_name
  resource_group_name = var.sqlserver_rg
}
