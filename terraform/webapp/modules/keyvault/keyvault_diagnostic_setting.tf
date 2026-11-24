resource "azurerm_monitor_diagnostic_setting" "keyvault_diagnostic" {
  name                       = "${var.project}-${var.environment}-keyvault-diagnostic"
  target_resource_id         = azurerm_key_vault.keyvault.id
  log_analytics_workspace_id = var.log_analytics_workspace_id

  enabled_log {
    category_group = "allLogs"
  }

  enabled_metric {
    category = "AllMetrics"
  }
}

resource "azurerm_monitor_diagnostic_setting" "core_keyvault" {
  name                       = "${var.project}-${var.core_env}-core-keyvault-diagnostic"
  target_resource_id         = var.keyvault_core_id
  log_analytics_workspace_id = var.core_log_analytics_workspace_id

  enabled_log {
    category_group = "allLogs"
  }

  enabled_metric {
    category = "AllMetrics"
  }
}
