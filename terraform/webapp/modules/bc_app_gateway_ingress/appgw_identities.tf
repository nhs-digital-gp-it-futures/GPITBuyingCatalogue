data "azurerm_user_assigned_identity" "managed_identity_aad" {
  name                = "${local.name_fragment}-managed-id"
  resource_group_name = "${local.name_fragment}-rg-webapp"
}
