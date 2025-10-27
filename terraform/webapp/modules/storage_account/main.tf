resource "azurerm_storage_account" "storage_account" {
  name                = var.storage_account_name
  resource_group_name = var.resource_group

  location                 = var.region
  account_tier             = "Standard"
  account_kind             = "StorageV2"
  account_replication_type = "GRS"
  shared_access_key_enabled = false

  min_tls_version            = "TLS1_2"
  https_traffic_only_enabled = true

  network_rules {
    default_action             = "Deny"
    ip_rules                   = var.ip_rules
    virtual_network_subnet_ids = var.subnet_ids
  }

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_role_assignment" "blob_data_assignment" {
  scope                = azurerm_storage_account.storage_account.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = var.principal_id
}

resource "azurerm_role_assignment" "queue_data_assignment" {
  scope                = azurerm_storage_account.storage_account.id
  role_definition_name = "Storage Queue Data Contributor"
  principal_id         = var.principal_id
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
