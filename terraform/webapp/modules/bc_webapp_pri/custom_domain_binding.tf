resource "azurerm_app_service_certificate" "webapp" {
  name                = "webapp_cert"
  resource_group_name = var.rg_name
  location            = var.region
  key_vault_secret_id = var.keyvault_cert_id
}

resource "azurerm_app_service_custom_hostname_binding" "webapp_host_binding" {
  hostname            = var.app_dns_url
  app_service_name    = azurerm_linux_web_app.webapp.name
  resource_group_name = var.rg_name
  count               = var.create_host_binding
}

resource "azurerm_app_service_certificate_binding" "webapp_cert_binding" {
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.webapp_host_binding.id
  ssl_state           = "SniEnabled"
  certificate_id      = var.ssl_cert_id
  count               = var.create_host_binding
}