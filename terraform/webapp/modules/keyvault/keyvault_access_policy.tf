data "azurerm_client_config" "current" { }

resource "azurerm_role_assignment" "keyvault_pipeline_access" {
  scope                = azurerm_key_vault.keyvault.id
  role_definition_name = "Key Vault Administrator"
  principal_id         = data.azurerm_client_config.current.object_id
}

resource "azurerm_role_assignment" "keyvault_aad_access" {
  scope                = var.keyvault_core_id
  role_definition_name = "Key Vault Administrator"
  principal_id         = var.principal_id
  principal_type       = "ServicePrincipal"
}
