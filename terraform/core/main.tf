resource "azurerm_resource_group" "general" {
  name     = "${var.project}-${var.environment}-core-rg"
  location = "uksouth"
}

resource "azurerm_management_lock" "general_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-core-rg-delete-protection"
  scope      = azurerm_resource_group.general.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the core resource group and its resources."
}

module "global-dns" {
  source              = "./modules/dns"
  domain_name         = var.domain_name
  environment         = var.environment
  resource_group_name = azurerm_resource_group.general.name
}
