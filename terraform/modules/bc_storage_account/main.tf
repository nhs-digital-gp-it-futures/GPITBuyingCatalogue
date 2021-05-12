resource "azurerm_storage_account" "storage_account" {
  name                      = var.storage_account_name
  location                  = var.region
  resource_group_name       = var.rg_name
  account_tier              = "Standard"
  account_replication_type  = var.replication_type
  account_kind              = "StorageV2"
  enable_https_traffic_only = "true"
  allow_blob_public_access  = "true"
  min_tls_version           = "TLS1_2"
  tags                      = {
    environment             = var.environment
  }
}
