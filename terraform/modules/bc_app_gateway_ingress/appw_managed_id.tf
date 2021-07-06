resource "azurerm_role_assignment" "managed_AG_Dev_Access" {
  scope                = azurerm_application_gateway.AppGw.id
  role_definition_name = "Contributor"
  principal_id         = var.managed_id_principal_id
}
