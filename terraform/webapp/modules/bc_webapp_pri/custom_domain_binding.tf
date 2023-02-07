resource "azurerm_app_service_custom_hostname_binding" "webapp_host_binding" {
  hostname            = var.app_dns_url
  app_service_name    = azurerm_linux_web_app.webapp.name
  resource_group_name = var.rg_name
  count               = var.create_host_binding
  ssl_state           = "SniEnabled"
  thumbprint          = var.ssl_thumbprint
}
