locals {
  secret_prefix = local.shortenv != "preprod" && local.shortenv != "production" ? "${var.pjtcode}${local.shortenv}" : "${var.pjtcode}${var.core_env}"
}

resource "random_password" "admin_password" {
  length            = 16
  special           = true
  override_special  = ".!-@"
  min_upper         = 1
  min_lower         = 1
  min_numeric       = 1
  min_special       = 1

  lifecycle {
    ignore_changes = [
      override_special,
    ]
  }
}

resource "azurerm_key_vault_secret" "sqladminusername" {
  name         = "${local.secret_prefix}sqladminusername"
  value        = var.kv_sqlusername
  content_type = "${var.project}-SQL-username"
  key_vault_id = azurerm_key_vault.keyvault.id

  tags = {
    environment = var.environment,
    architecture = "new"
  }

  depends_on = [azurerm_key_vault_access_policy.keyvault_pipeline_access]
}

resource "azurerm_key_vault_secret" "sqladminpassword" {
  name         = "${local.secret_prefix}sqladminpassword"
  value        = random_password.admin_password.result
  content_type = "${var.project}-SQL-password"  
  key_vault_id = azurerm_key_vault.keyvault.id

  tags = {
    environment = var.environment,
    architecture = "new"
  }

  depends_on = [azurerm_key_vault_access_policy.keyvault_pipeline_access]
}