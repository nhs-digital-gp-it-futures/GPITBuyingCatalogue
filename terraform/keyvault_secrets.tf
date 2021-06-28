resource "azurerm_key_vault_secret" "sqladminusername" {
  name         = "${local.secretPrefix}sqladminusername"
  value        = var.kv_sqlusername
  content_type = "${var.project}-SQL-username"
  key_vault_id = azurerm_key_vault.keyvault.id

  tags = {
    environment = var.environment,
    architecture = "new"
  }
}

resource "azurerm_key_vault_secret" "sqladminpassword" {
  name         = "${local.secretPrefix}sqladminpassword"
  value        = random_password.password.result
  content_type = "${var.project}-SQL-password"  
  key_vault_id = azurerm_key_vault.keyvault.id

  tags = {
    environment = var.environment,
    architecture = "new"
  }
}

resource "random_password" "password" {
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