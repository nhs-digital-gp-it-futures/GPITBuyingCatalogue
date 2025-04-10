data "azurerm_virtual_network" "infrastructure_vnet" {
  name                = "${var.project}-infra-vnet"
  resource_group_name = "${var.project}-rg-sa"
  provider            = azurerm.infrastructure
}

data "azurerm_subnet" "default-subnet" {
  resource_group_name  = data.azurerm_virtual_network.infrastructure_vnet.resource_group_name
  virtual_network_name = data.azurerm_virtual_network.infrastructure_vnet.name
  name                 = "default"
  provider             = azurerm.infrastructure
}