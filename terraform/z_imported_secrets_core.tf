data "azurerm_key_vault" "keyvault_core" {
  name                = "${var.project}-${local.coreEnv}-core-kv"
  resource_group_name = "${var.project}-${local.coreEnv}-rg-kv"
}

data "azurerm_key_vault_secret" "ssl_cert" {
  name         = "${var.certname}-star"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "spn_appid" {
 name         = "${var.pjtcode}${local.coreEnv}spnapplicationid"
 key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "sqladmins" {
  name         = "${var.pjtcode}${local.coreEnv}SG-SQLAdmins"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}
