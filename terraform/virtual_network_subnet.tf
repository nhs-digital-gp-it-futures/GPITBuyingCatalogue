resource "azurerm_subnet" "gateway" {
  name                 = "${var.project}-${var.environment}-gateway-subnet"
  resource_group_name  = azurerm_resource_group.virtualnet.name
  virtual_network_name = azurerm_virtual_network.virtualnet.name
  address_prefixes     = [var.vnet_address_space]

  service_endpoints    = ["Microsoft.Sql","Microsoft.Storage"]
}