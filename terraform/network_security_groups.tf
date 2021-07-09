resource "azurerm_network_security_group" "gateway" {
  name                = "${var.project}-${var.environment}-nsg-appgateway"
  location            = var.region
  resource_group_name = azurerm_resource_group.app-gateway.name

  tags = {
    environment       = var.environment,
    architecture = "new"
  }
}
