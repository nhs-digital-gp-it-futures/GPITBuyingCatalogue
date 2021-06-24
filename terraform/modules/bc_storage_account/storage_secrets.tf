resource "azurerm_key_vault_secret" "kv_sa_string" {
  name         = var.kv_key 
  value        = azurerm_storage_account.storageaccount.primary_connection_string
  content_type = "${var.project} Storage Account Connection String"
  key_vault_id = var.kv_id
  
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}
