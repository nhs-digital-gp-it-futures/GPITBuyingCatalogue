data "azurerm_virtual_network" "primary_virtualnet" {
  name                = "primary-virtualnet"
  resource_group_name = "${var.project}-${var.primary_env}-rg-virtualnet"
}

resource "azurerm_virtual_network_peering" "secondary-to-primary-vnet" {
  name                      = "${var.project}-${var.environment}-vnet-peer"
  resource_group_name       = azurerm_resource_group.virtualnet.name
  virtual_network_name      = azurerm_virtual_network.virtualnet.name
  remote_virtual_network_id = azurerm_virtual_network.primary_virtualnet.id
}
