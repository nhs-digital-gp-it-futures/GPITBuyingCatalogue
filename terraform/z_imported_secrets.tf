data "azurerm_key_vault_secret" "coreurl" {
  name         = "${local.secretPrefix}coreurl"
  key_vault_id = local.kv_id
}

data "azurerm_key_vault_secret" "certname" {
  name         = "${local.secretPrefix}certname"
  key_vault_id = local.kv_id
}
