data "azurerm_sql_server" "sql_server" {
  name                = "${var.project}-${var.environment}-sql-pri"
  resource_group_name = "${var.project}-${var.environment}-rg-sql-pri"
}
