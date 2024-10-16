resource "azurerm_dns_zone" "dns-zone" {
  name                = var.domain_name
  resource_group_name = var.resource_group_name
  tags = {
    environment = var.environment
  }
}
