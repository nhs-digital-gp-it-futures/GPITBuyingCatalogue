data "azurerm_key_vault" "keyvault_core" {
  name                = "${var.project}-${var.core_environment}-core-kv"
  resource_group_name = "${var.project}-${var.core_environment}-rg-kv"
}

data "azurerm_key_vault_secret" "ssl_cert" {
  name         = "${var.cert_name}-star"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "bjssvpn" {
  name         = "${var.pjtcode}${var.core_environment}bjssvpn"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "mastekvpn1" {
  name         = "${var.pjtcode}${var.core_environment}mastekvpn1"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "mastekvpn2" {
  name         = "${var.pjtcode}${var.core_environment}mastekvpn2"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_secret" "nhsdoffice1" {
  name         = "${var.pjtcode}${var.core_environment}nhsdoffice1"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}
