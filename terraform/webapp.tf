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
  auth_pwd         = data.azurerm_key_vault_secret.sqladminpassword.value
  cert_name        = data.azurerm_key_vault_secret.certname.value
  webapp_cname_url = local.gw_webappURL
  core_environment = local.coreEnv
}
