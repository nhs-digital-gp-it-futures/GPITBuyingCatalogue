output "primary_connection_string" {
    description = "The primary connection string for the storage account"
    value = azurerm_storage_account.storage_account.primary_connection_string
    sensitive   = true
}
