data "azurerm_user_assigned_identity" "managed_identity_aad" {
  name                = "${var.ag_name_fragment}-managed-id"
  resource_group_name = "${var.ag_name_fragment}-rg-webapp"
}
