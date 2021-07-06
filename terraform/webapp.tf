data "azurerm_container_registry" "acr" {
  name                = "gpitfuturesdevacr"
  resource_group_name = "gpitfutures-dev-rg-acr"
}

module "webapp" {
  source           = "./modules/bc_webapp_pri"
  
  environment      = var.environment
  region           = var.region
  project          = var.project
  pjtcode          = var.pjtcode
  rg_name          = azurerm_resource_group.webapp.name
  webapp_name      = "${var.project}-${var.environment}-webapp"
  sku_tier         = local.shortenv != "preprod" && local.shortenv != "production" ? "Basic" : "Standard"
  sku_size         = local.shortenv != "preprod" && local.shortenv != "production" ? "B1" : "S1"
  acr_name         = "gpitfuturesdevacr"
  acr_pwd          = data.azurerm_container_registry.acr.admin_password
  acr_rg           = "gpitfutures-dev-rg-acr"
  repository_name  = "nhsd/buying-catalogue/nhsdgpitbuyingcataloguewebapp"
  always_on        = local.shortenv != "production" ? "false" : "true"
  db_name_main     = module.sql_databases_pri.sql_main_dbname # in cluster "bc-${var.environment}-bapi"  
  auth_pwd         = azurerm_key_vault_secret.sqladminpassword.value
  cert_name        = var.certname
  webapp_cname_url = local.gw_webappURL
  sa_connection_string = module.storage_account.primary_connection_string
  aspnet_environment = "Development"
  sqlserver_name = module.sql_server_pri.sql_server_name
  sqlserver_rg = azurerm_resource_group.sql-primary.name
  instrumentation_key = azurerm_application_insights.appinsights.instrumentation_key
  primary_vpn = var.primary_vpn
  secondary_vpn = var.secondary_vpn
  tertiary_vpn = var.tertiary_vpn
  ssl_cert = data.azurerm_key_vault_secret.ssl_cert.value
  customer_network_range = var.nhsd_network_range
  smtp_server_host = var.smtp_server_host
  smtp_server_port = var.smtp_server_port
  
  depends_on = [module.sql_server_pri]
}
