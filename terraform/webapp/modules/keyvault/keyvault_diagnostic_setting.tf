resource "azurerm_monitor_diagnostic_setting" "keyvault_diagnostic" {
  name               = "${var.project}-${var.environment}-keyvault-diagnostic"
  target_resource_id = azurerm_key_vault.keyvault.id
  log_analytics_workspace_id = var.log_analytics_workspace_id

  enabled_log {
    category_group = "allLogs"
  }

  enabled_metric {
    category = "AllMetrics"
  }
}