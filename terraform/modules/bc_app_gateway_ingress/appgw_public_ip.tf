resource "azurerm_public_ip" "PipAppGw" {
  name                = var.pip_name
  location            = var.region
  domain_name_label   = var.dns_name
  resource_group_name = var.rg_name
  allocation_method   = "Static"
  sku                 = "Standard"

  tags = {
    environment       = var.environment
  }
}
