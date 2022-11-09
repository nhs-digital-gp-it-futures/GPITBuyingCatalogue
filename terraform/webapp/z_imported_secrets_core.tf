data "azurerm_key_vault" "keyvault_core" {
  name                = "gpitf-${local.core_env}-core-kv"
  resource_group_name = "${var.project}-${local.core_env}-rg-core-kv"
}

data "azurerm_key_vault_secret" "ssl_cert" {
  name         = "${var.certname}-star"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault_certificate" "ssl_cert" {
  name         = "${var.certname}-star"
  key_vault_id = data.azurerm_key_vault.keyvault_core.id
}

data "azurerm_key_vault" "keyvault_target" {
  count               = local.is_dr ? 1 : 0
  name                = "gpitf-${var.primary_env}-kv"
  resource_group_name = "${var.project}-${var.primary_env}-rg-keyvault"
}

data "azurerm_key_vault_secret" "sqladminusername" {
  count         = local.is_dr ? 1 : 0
  name          = "${var.pjtcode}${var.primary_env}sqladminusername"
  key_vault_id  = join("", data.azurerm_key_vault.keyvault_target[*].id)
}

data "azurerm_key_vault_secret" "sqladminpassword" {
  count         = local.is_dr ? 1 : 0
  name          = "${var.pjtcode}${var.primary_env}sqladminpassword"
  key_vault_id  = join("", data.azurerm_key_vault.keyvault_target[*].id)
}

data "azurerm_key_vault_secret" "sqlhangfireusername" {
  count         = local.is_dr ? 1 : 0
  name          = "${var.pjtcode}${var.primary_env}sqlhangfireusername"
  key_vault_id  = join("", data.azurerm_key_vault.keyvault_target[*].id)
}

data "azurerm_key_vault_secret" "sqlhangfirepassword" {
  count         = local.is_dr ? 1 : 0
  name          = "${var.pjtcode}${var.primary_env}sqlhangfirepassword"
  key_vault_id  = join("", data.azurerm_key_vault.keyvault_target[*].id)
}