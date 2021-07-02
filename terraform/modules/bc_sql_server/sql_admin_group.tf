data "azurerm_client_config" "current" {
}


resource "azurerm_sql_active_directory_administrator" "sql_admins" {
  server_name         = azurerm_sql_server.sql_server.name
  resource_group_name = var.rg_name # azurerm_resource_group.sql-pri.name
  login               = var.sqladmins # 
  tenant_id           = data.azurerm_client_config.current.tenant_id
  object_id           = var.sqladmins # data.azurerm_key_vault_secret.sqladmins.value 
}
