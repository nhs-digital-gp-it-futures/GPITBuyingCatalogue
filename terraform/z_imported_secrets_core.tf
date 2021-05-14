data "azurerm_key_vault" "keyvault_core" {
  name                = "${var.project}-${local.coreEnv}-core-kv"
  resource_group_name = "${var.project}-${local.coreEnv}-rg-kv"
}

data "azurerm_key_vault_secret" "ssl_cert" {
  name         = "${data.azurerm_key_vault_secret.certname.value}-star"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "bjssvpn" {
  name         = "${var.pjtcode}${local.coreEnv}bjssvpn"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "mastekvpn1" {
  name         = "${var.pjtcode}${local.coreEnv}mastekvpn1"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "mastekvpn2" {
  name         = "${var.pjtcode}${local.coreEnv}mastekvpn2"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}
