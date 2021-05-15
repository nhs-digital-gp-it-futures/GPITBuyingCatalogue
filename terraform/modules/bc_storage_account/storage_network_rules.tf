resource "azurerm_storage_account_network_rules" "storage_fw" {
  resource_group_name  = var.rg_name
  storage_account_name = azurerm_storage_account.storage_account.name

  default_action             = "Deny"
  ip_rules                   = var.ip_rules
  virtual_network_subnet_ids = [var.aks_subnet_id]
  bypass                     = ["AzureServices"]
}
