resource "azurerm_resource_group" "general" {
  name     = "${var.project}-${var.environment}-core-rg"
  location = "uksouth"
}

module "global-dns" {
  source              = "./modules/dns"
  domain_name         = var.domain_name
  environment         = var.environment
  resource_group_name = azurerm_resource_group.general.name
}
