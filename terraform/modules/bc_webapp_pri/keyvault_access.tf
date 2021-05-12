data "azuread_service_principal" "MicrosoftWebApp" {
  application_id = "abfa0a7c-a6b6-4736-8310-5855508787cd"
}

data "azurerm_client_config" "current" {
}

resource "azurerm_key_vault_access_policy" "keyvault_aad_access" {
  key_vault_id   = data.azurerm_key_vault.keyvault_core.id
  tenant_id      = data.azurerm_client_config.current.tenant_id
  object_id      = data.azuread_service_principal.MicrosoftWebApp.object_id

  key_permissions = [
  ]

  secret_permissions = [
    "get",
  ]

  certificate_permissions = [
    "get",
  ]
}
