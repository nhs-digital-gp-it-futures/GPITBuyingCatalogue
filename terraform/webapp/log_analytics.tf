resource "azurerm_log_analytics_workspace" "log_analytics" {
  name                = "${var.project}-${var.environment}-log-analytics"
  location            = var.region
  resource_group_name = azurerm_resource_group.log-analytics
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}
