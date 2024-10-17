module "waf_policy" {
  source              = "./modules/waf_policy"
  count               = local.core_env != "dev" ? 1 : 0
  resource_group_name = azurerm_resource_group.app-gateway.name
  project_name        = var.project
  environment         = var.environment
  location            = var.region
}

module "appgateway" {
  source                     = "./modules/bc_app_gateway_ingress"
  project                    = var.project
  environment                = var.environment
  region                     = var.region
  rg_name                    = azurerm_resource_group.app-gateway.name
  ag_capacity                = local.shortenv != "preprod" && local.shortenv != "production" ? "1" : "2"
  ag_subnet_id               = azurerm_subnet.gateway.id
  core_env                   = local.core_env
  ssl_cert_name              = var.certname
  ssl_cert_secret_id = trimsuffix(data.azurerm_key_vault_secret.ssl_cert.id, data.azurerm_key_vault_secret.ssl_cert.version)
  managed_id_principal_id    = azurerm_user_assigned_identity.managed_id.principal_id
  dns_name                   = "gpitfbuyingcatalogue${local.shortenv}"
  app_service_hostname       = module.webapp.webapp_default_site_hostname
  app_dns_url                = var.app_url
  log_analytics_workspace_id = azurerm_log_analytics_workspace.log_analytics.id
  firewall_policy_id = join("", module.waf_policy[*].firewall_policy_id)

  depends_on = [
    azurerm_user_assigned_identity.managed_id
  ]
}
