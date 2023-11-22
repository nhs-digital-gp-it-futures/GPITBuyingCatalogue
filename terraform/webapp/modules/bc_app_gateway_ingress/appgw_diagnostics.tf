resource "azurerm_monitor_diagnostic_setting" "example" {
  name               = local.diagnostics_name
  target_resource_id = azurerm_application_gateway.app_gateway[0].id
  storage_account_id = var.storage_account_id
  count              = var.core_env != "dev" ? 1 : 0

  dynamic "enabled_log" {
    for_each = local.gateway_logs
    content {
      category = enabled_log.value
    }
  }
}