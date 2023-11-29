module "diagnostics" {
  source                     = "../bc_diagnostics_setting"
  name                       = local.diagnostics_name
  enable_logs                = local.enable_logs
  log_analytics_workspace_id = var.log_analytics_workspace_id
  target_resource_id         = azurerm_mssql_server.sql_server.id 
} 
