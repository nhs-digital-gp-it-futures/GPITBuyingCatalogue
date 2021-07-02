output "primary_connection_string" {
    description = "The primary connection string for the storage account"
    value = azurerm_storage_account.storageaccount.primary_connection_string
    sensitive   = true
}
