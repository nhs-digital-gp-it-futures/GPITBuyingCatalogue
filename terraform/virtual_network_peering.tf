data "azurerm_virtual_network" "primary_virtualnet" {
  name                = "gpitfutures-${var.primary_env}-virtualnet"
  count               = var.environment == var.primary_env ? 0 : 1
  resource_group_name = "${var.project}-${var.primary_env}-rg-virtualnet"
}

resource "azurerm_virtual_network_peering" "secondary-to-primary-vnet" {
  name                      = "${var.project}-${var.environment}-vnet-peer"
  count                     = var.environment == var.primary_env ? 0 : 1
  resource_group_name       = azurerm_resource_group.virtualnet.name
  virtual_network_name      = azurerm_virtual_network.virtualnet.name
  remote_virtual_network_id = data.azurerm_virtual_network.primary_virtualnet[0].id
}
