resource "azurerm_virtual_network" "virtualnet" {
  name                = "${var.project}-${var.environment}-virtualnet"
  location            = var.region
  address_space       = [var.vnet_address_space]
  resource_group_name = azurerm_resource_group.virtualnet.name

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}
