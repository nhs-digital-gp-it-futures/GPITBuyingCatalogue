resource "azurerm_monitor_diagnostic_setting" "diagnostic-setting" {
  name                       = var.name
  target_resource_id         = var.target_resource_id
  log_analytics_workspace_id = var.log_analytics_workspace_id

  dynamic "enabled_log" {
    for_each = var.enable_logs
    content {
      category = enabled_log.value
    }
  }
}
