resource "azurerm_log_analytics_workspace" "log_analytics" {
  name                = "${var.project}-${var.environment}-log-analytics"
  location            = var.region
  resource_group_name = azurerm_resource_group.log-analytics.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_log_analytics_workspace" "core_keyvault" {
  name                = "${var.project}-${local.core_env}-core-keyvault-logs"
  location            = data.azurerm_key_vault.keyvault_core.location
  resource_group_name = data.azurerm_key_vault.keyvault_core.resource_group_name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = {
    environment = local.core_env
  }
}
