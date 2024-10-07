resource "azurerm_network_security_group" "gateway" {
  name                = "${var.project}-${var.environment}-nsg-appgateway"
  location            = var.region
  resource_group_name = azurerm_resource_group.app-gateway.name

  tags = {
    environment       = var.environment,
    architecture = "new"
  }
}
resource "azurerm_subnet_network_security_group_association" "gateway" {
  subnet_id                 = azurerm_subnet.gateway.id
  network_security_group_id = azurerm_network_security_group.gateway.id

  depends_on = [
    azurerm_network_security_rule.Azure
  ]
}
