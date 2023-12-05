module "diagnostics" {
  source                     = "../bc_diagnostics_setting"
  name                       = local.diagnostics_name
  enable_logs                = local.enable_logs
  log_analytics_workspace_id = var.log_analytics_workspace_id
  target_resource_id         = local.target_resource_id
} 

resource "azurerm_mssql_server_extended_auditing_policy" "auditing" {
  server_id              = azurerm_mssql_server.sql_server.id 
  log_monitoring_enabled = true
}
