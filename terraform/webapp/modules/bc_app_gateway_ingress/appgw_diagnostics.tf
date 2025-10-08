module "diagnostics" {
  source                     = "../bc_diagnostics_setting"
  name                       = local.diagnostics_name
  enable_logs                = local.gateway_logs
  enable_metrics             = local.gateway_metrics
  log_analytics_workspace_id = var.log_analytics_workspace_id
  target_resource_id         = azurerm_application_gateway.app_gateway[0].id
  count                      = var.use_app_gateway ? 1 : 0
} 
