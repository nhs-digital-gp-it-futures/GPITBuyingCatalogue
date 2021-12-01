module "webapp" {
  source           = "./modules/bc_webapp_pri"
  
  environment      = var.environment
  region           = var.region
  project          = var.project
  pjtcode          = var.pjtcode
  rg_name          = azurerm_resource_group.webapp.name
  webapp_name      = "${var.project}-${var.environment}-webapp"
  sku_tier         = "Standard"
  sku_size         = local.shortenv != "production" ? "S1" : "S2"
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
  tertiary_vpn = module.appgateway.appgateway_pip_ipaddress
  ssl_cert = data.azurerm_key_vault_secret.ssl_cert.value
  customer_network_range = var.nhsd_network_range
  smtp_server_host = var.smtp_server_host
  smtp_server_port = var.smtp_server_port
  vnet_subnet_id = azurerm_subnet.gateway.id
  app_dns_url = var.app_url
  docker_registry_server_url = data.azurerm_container_registry.acr.login_server
  docker_registry_server_username = data.azurerm_container_registry.acr.admin_username
  docker_registry_server_password = data.azurerm_container_registry.acr.admin_password

  depends_on = [module.sql_server_pri]
}
