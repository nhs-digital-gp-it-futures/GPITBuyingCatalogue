resource "azurerm_app_service_virtual_network_swift_connection" "webapp" {
  app_service_id = module.webapp.webapp_service_id
  subnet_id      = azurerm_subnet.backend.id
}
