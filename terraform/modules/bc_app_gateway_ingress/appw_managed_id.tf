resource "azurerm_role_assignment" "managed_ag_access" {
  scope                = azurerm_application_gateway.app_gateway[0].id
  role_definition_name = "Contributor"
  principal_id         = var.managed_id_principal_id
  count                = var.core_env != "dev" ? 1 : 0
}
