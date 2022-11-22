data "azurerm_application_insights" "app_insights" {
  name                = "${local.project_environment}-appinsights"
  resource_group_name = "${local.project_environment}-rg-appinsights"
}

data "azurerm_mssql_server" "buyingcataloguedb" {
  name                = "${local.project_environment}-sql-primary"
  resource_group_name = "${local.project_environment}-rg-sql-server"
}

data "azurerm_key_vault" "buyingcataloguekv" {
  name                = "${local.project_short_code}-${var.environment}-kv"
  resource_group_name = "${local.project_environment}-rg-keyvault"
}

data "azurerm_key_vault_secret" "sqladminusername" {
  name          = "${local.project_alt_code}${var.environment}sqladminusername"
  key_vault_id  = data.azurerm_key_vault.buyingcataloguekv.id
}

data "azurerm_key_vault_secret" "sqladminpassword" {
  name          = "${local.project_alt_code}${var.environment}sqladminpassword"
  key_vault_id  = data.azurerm_key_vault.buyingcataloguekv.id
}