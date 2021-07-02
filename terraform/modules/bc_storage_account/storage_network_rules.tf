resource "azurerm_storage_account_network_rules" "storage_fw" {
  resource_group_name  = var.rg_name
  storage_account_name = azurerm_storage_account.storageaccount.name

  default_action             = "Allow"
  /*ip_rules                   = var.ip_rules
  virtual_network_subnet_ids = [var.vnet_subnet_id]
  bypass                     = ["AzureServices"]*/
}
