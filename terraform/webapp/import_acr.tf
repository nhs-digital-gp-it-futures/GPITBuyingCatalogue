data "azurerm_container_registry" "acr" {
  provider            = azurerm.acr
  name                = "gpitfuturesdevacr"
  resource_group_name = "gpitfutures-dev-rg-acr"
}
