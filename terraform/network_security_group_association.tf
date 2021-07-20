resource "azurerm_subnet_network_security_group_association" "gateway" {
  subnet_id                 = azurerm_subnet.gateway.id
  network_security_group_id = azurerm_network_security_group.gateway.id

  depends_on = [
    azurerm_network_security_rule.Azure
  ]
 }
