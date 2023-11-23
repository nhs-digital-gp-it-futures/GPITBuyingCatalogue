module "webapp" {
  source = "./modules/bc_webapp_pri"

  environment                     = var.environment
  region                          = var.region
  project                         = var.project
  rg_name                         = azurerm_resource_group.webapp.name
  webapp_name                     = "${var.project}-${var.environment}-webapp"
  sku_tier                        = local.shortenv == "preprod" || local.shortenv == "production" ? "PremiumV2" : "Standard"
  sku_size                        = local.shortenv == "preprod" || local.shortenv == "production" ? "P2v2" : "S1"
  repository_name                 = "nhsd/buying-catalogue/nhsdgpitbuyingcataloguewebapp"
  always_on                       = local.shortenv != "production" ? "false" : "true"
  cert_name                       = var.certname
  aspnet_environment              = var.environment
  instrumentation_key             = azurerm_application_insights.appinsights.instrumentation_key
  primary_vpn                     = var.primary_vpn
  app_gateway_ip                  = module.appgateway.appgateway_pip_ipaddress
  app_dns_url                     = var.app_url
  docker_registry_server_url      = data.azurerm_container_registry.acr.login_server
  docker_registry_server_username = data.azurerm_container_registry.acr.admin_username
  docker_registry_server_password = data.azurerm_container_registry.acr.admin_password
  create_slot                     = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0
  create_host_binding             = local.core_env == "dev" ? 1 : 0
  ssl_thumbprint                  = data.azurerm_key_vault_certificate.ssl_cert.thumbprint
  notify_api_key                  = var.notify_api_key
  blob_storage_connection_string  = module.documentstorageaccount.primary_connection_string
  recaptcha_site_key              = var.recaptcha_site_key
  recaptcha_secret_key            = var.recaptcha_secret_key
  identity                        = azurerm_user_assigned_identity.managed_webapp_id.id

  # SQL Vars
  sqlserver_name     = local.is_dr ? "${var.project}-${var.primary_env}-sql-primary" : join("", module.sql_server_pri[*].sql_server_name)
  sqlserver_rg       = local.is_dr ? "${var.project}-${var.primary_env}-rg-sql-server" : azurerm_resource_group.sql-server.name
  db_name_main       = local.is_dr ? "BuyingCatalogue-${var.primary_env}" : join("", module.sql_databases_pri[*].sql_main_dbname) # in cluster "bc-${var.environment}-bapi"
  sql_admin_username = local.is_dr ? data.azurerm_key_vault_secret.sqladminusername[0].value : join("", module.keyvault[*].sqladminusername)
  sql_admin_password = local.is_dr ? data.azurerm_key_vault_secret.sqladminpassword[0].value : join("", module.keyvault[*].sqladminpassword)
  depends_on         = [module.sql_server_pri]
}
