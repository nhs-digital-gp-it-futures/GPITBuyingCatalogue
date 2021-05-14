resource "azurerm_key_vault_secret" "kv_sa_string" {
  name         = var.kv_key 
  value        = azurerm_storage_account.storage_account.primary_connection_string
  content_type = "${var.project}-Connection-String"
  key_vault_id = var.kv_id
  
  tags = {
    environment = var.environment
  }
}
