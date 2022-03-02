resource "azurerm_key_vault_secret" "sqladminusername" {
  name         = "${local.secretPrefix}sqladminusername"
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
  name         = "${local.secretPrefix}sqladminpassword"
  value        = random_password.admin_password.result
  content_type = "${var.project}-SQL-password"  
  key_vault_id = azurerm_key_vault.keyvault.id

  tags = {
    environment = var.environment,
    architecture = "new"
  }

  depends_on = [azurerm_key_vault_access_policy.keyvault_pipeline_access]
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

resource "azurerm_key_vault_secret" "sqlhangfireusername" {
  name         = "${local.secretPrefix}sqlhangfireusername"
  value        = var.kv_sql_hangfire_username
  content_type = "${var.project}-SQL-hangfire-username"
  key_vault_id = azurerm_key_vault.keyvault.id

  tags = {
    environment = var.environment,
    architecture = "new"
  }

  depends_on = [azurerm_key_vault_access_policy.keyvault_pipeline_access]
}

resource "azurerm_key_vault_secret" "sqlhangfirepassword" {
  name         = "${local.secretPrefix}sqlhangfirepassword"
  value        = random_password.hangfire_password.result
  content_type = "${var.project}-SQL-hangfire-password"  
  key_vault_id = azurerm_key_vault.keyvault.id

  tags = {
    environment = var.environment,
    architecture = "new"
  }

  depends_on = [azurerm_key_vault_access_policy.keyvault_pipeline_access]
}

resource "random_password" "hangfire_password" {
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