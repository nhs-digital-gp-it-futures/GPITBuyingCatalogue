resource "azurerm_app_service_certificate" "webapp" {
  name                = "webapp_cert"
  resource_group_name = azurerm_resource_group.webapp.name
  location            = var.region
  key_vault_secret_id = data.azurerm_key_vault_secret.ssl_cert.id
}

resource "azurerm_app_service_custom_hostname_binding" "webapp" {
  hostname            = var.coreurl
  app_service_name    = module.webapp.webapp_name
  resource_group_name = azurerm_resource_group.webapp.name
  count = local.is_dr ? 1 : 0

  depends_on = [
    module.webapp,
  ]
}

resource "azurerm_app_service_certificate_binding" "webapp" {
  hostname_binding_id = join("", azurerm_app_service_custom_hostname_binding.webapp[*].id)
  certificate_id      = azurerm_app_service_certificate.webapp.id
  ssl_state           = "SniEnabled"
  count = local.is_dr ? 1 : 0
  
  depends_on = [
    azurerm_app_service_certificate.webapp,
    azurerm_app_service_custom_hostname_binding.webapp
  ]
}
