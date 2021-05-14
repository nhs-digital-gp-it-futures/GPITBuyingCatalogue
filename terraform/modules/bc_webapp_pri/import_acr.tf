data "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = var.acr_rg
}
