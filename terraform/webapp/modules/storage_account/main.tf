resource "azurerm_storage_account" "storage_account" {
  name                = var.storage_account_name
  resource_group_name = var.resource_group

  location                 = var.region
  account_tier             = "Standard"
  account_kind             = "StorageV2"
  account_replication_type = "GRS"

  min_tls_version               = "TLS1_2"
  https_traffic_only_enabled    = true

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_key_vault_secret" "storageaccount_connectionstring" {
  name         = azurerm_storage_account.storage_account.name
  value        = azurerm_storage_account.storage_account.primary_connection_string
  key_vault_id = var.key_vault_id

  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  depends_on = [azurerm_storage_account.storage_account]
}

output "storage_account_id" {
  description = "Storage account id"
  value       = azurerm_storage_account.storage_account.id
}

output "storage_account_name" {
  description = "Storage account name"
  value       = azurerm_storage_account.storage_account.name
}

output "primary_connection_string" {
  description = "Storage account connection string"
  value       = azurerm_storage_account.storage_account.primary_connection_string
  sensitive   = true
}
