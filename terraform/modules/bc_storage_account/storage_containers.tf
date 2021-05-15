resource "azurerm_storage_container" "documents" {
  name                  = var.container_name
  storage_account_name  = azurerm_storage_account.storage_account.name
  container_access_type = "blob"
}
