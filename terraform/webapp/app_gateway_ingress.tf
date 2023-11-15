module "appgateway" {
  source                  = "./modules/bc_app_gateway_ingress"

  environment             = var.environment
  region                  = var.region
  rg_name                 = azurerm_resource_group.app-gateway.name
  diag_name               = "${var.project}-${var.environment}-appgwdiag"
  ag_name                 = "${var.project}-${var.environment}-appgateway"
  ag_name_fragment        = "${var.project}-${var.environment}"
  ag_capacity             = local.shortenv != "preprod" && local.shortenv != "production" ? "1" : "2"
  ag_subnet_id            = azurerm_subnet.gateway.id
  core_env                = local.core_env
  core_url                = var.coreurl
  ssl_cert_name           = var.certname
  ssl_cert_secret_id      = trimsuffix(data.azurerm_key_vault_secret.ssl_cert.id, data.azurerm_key_vault_secret.ssl_cert.version)
  managed_id_principal_id = azurerm_user_assigned_identity.managed_id.principal_id
  dns_name                = "gpitfbuyingcatalogue${local.shortenv}"
  pip_name                = "${var.project}-${var.environment}-publicip"
  app_service_hostname    = module.webapp.webapp_default_site_hostname
  app_dns_url             = var.app_url
  storage_account_id      = module.documentstorageaccount.storage_account_id
  
  depends_on = [
    azurerm_user_assigned_identity.managed_id
  ]
}
