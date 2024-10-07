resource "azurerm_subnet" "gateway" {
  name                              = "${var.project}-${var.environment}-gateway-subnet"
  private_endpoint_network_policies = "Enabled"
  resource_group_name               = azurerm_resource_group.virtualnet.name
  virtual_network_name              = azurerm_virtual_network.virtualnet.name
  address_prefixes = [var.vnet_gateway_address_space]
}

resource "azurerm_subnet" "backend" {
  name                              = "${var.project}-${var.environment}-backend-subnet"
  private_endpoint_network_policies = "Enabled"
  resource_group_name               = azurerm_resource_group.virtualnet.name
  virtual_network_name              = azurerm_virtual_network.virtualnet.name
  address_prefixes = [var.vnet_backend_address_space]

  delegation {
    name = "${var.project}-${var.environment}-backend-delegation"

    service_delegation {
      name = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }

  service_endpoints = ["Microsoft.Sql", "Microsoft.Storage"]
}
