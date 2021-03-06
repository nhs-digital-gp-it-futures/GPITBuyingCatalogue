data "azurerm_key_vault" "keyvault_core" {
  name                = "gpitf-${local.coreEnv}-core-kv"
  resource_group_name = "${var.project}-${local.coreEnv}-rg-core-kv"
}

data "azurerm_key_vault_secret" "ssl_cert" {
  name         = "${var.certname}-star"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_certificate" "ssl_cert" {
  name         = "${var.certname}-star"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}
