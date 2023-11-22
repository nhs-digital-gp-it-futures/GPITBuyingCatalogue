resource "azurerm_public_ip" "pip_app_gateway" {
  name                = local.public_ip_name
  location            = var.region
  domain_name_label   = var.dns_name
  resource_group_name = var.rg_name
  allocation_method   = "Static"
  sku                 = "Standard"
  count               = var.core_env != "dev" ? 1 : 0
  zones               = ["1", "2", "3"]
  tags = {
    environment       = var.environment,
    architecture      = "new"
  }
}
