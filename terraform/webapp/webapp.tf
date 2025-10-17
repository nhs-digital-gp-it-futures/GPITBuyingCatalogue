module "webapp" {
  source = "./modules/bc_webapp_pri"

  environment                     = var.environment
  region                          = var.region
  project                         = var.project
  rg_name                         = azurerm_resource_group.webapp.name
  webapp_name                     = "${var.project}-${var.environment}-webapp"
  sku_tier                        = local.web_sku_tier
  sku_size                        = local.web_sku_size
  repository_name                 = "nhsd/buying-catalogue/nhsdgpitbuyingcataloguewebapp"
  always_on                       = local.shortenv == "production" ? "true" : "false"
  aspnet_environment              = var.environment
  instrumentation_key             = azurerm_application_insights.appinsights.instrumentation_key
  vpn                             = concat(var.primary_vpn, var.secondary_vpn)
  app_gateway_ip                  = module.appgateway.appgateway_pip_ipaddress
  app_dns_url                     = var.app_url
  docker_registry_server_url      = data.azurerm_container_registry.acr.login_server
  docker_registry_id              = data.azurerm_container_registry.acr.id
  create_slot                     = local.is_live_environment ? 1 : 0
  create_host_binding             = !local.use_app_gateway ? 1 : 0
  keyvault_cert_id                = data.azurerm_key_vault_secret.ssl_cert.id
  notify_api_key                  = var.notify_api_key
  storage_account_name            = module.documentstorageaccount.storage_account_name
  recaptcha_site_key              = var.recaptcha_site_key
  recaptcha_secret_key            = var.recaptcha_secret_key
  backend_subnet_id               = azurerm_subnet.backend.id
  webapp_identity                 = azurerm_user_assigned_identity.web_app_identity

  # SQL Vars
  sqlserver_name     = join("", module.sql_server_pri[*].sql_server_name)
  sqlserver_rg       = azurerm_resource_group.sql-server.name
  db_name_main       = join("", module.sql_databases_pri[*].sql_main_dbname) # in cluster "bc-${var.environment}-bapi"
  sql_admin_username = join("", module.keyvault[*].sqladminusername)
  sql_admin_password = join("", module.keyvault[*].sqladminpassword)
  depends_on         = [module.sql_server_pri]
}
