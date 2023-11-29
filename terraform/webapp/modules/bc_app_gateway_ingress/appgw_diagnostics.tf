module "diagnostics" {
  source                     = "../bc_diagnostics_setting"
  name                       = local.diagnostics_name
  enable_logs                = local.gateway_logs
  log_analytics_workspace_id = var.log_analytics_workspace_id
  target_resource_id         = azurerm_application_gateway.app_gateway[0].id
  count                      = var.core_env != "dev" ? 1 : 0
} 
