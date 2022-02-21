module "sql_server_pri" {
  source                = "./modules/bc_sql_server"
  
  environment           = var.environment
  region                = var.region
  project               = var.project
  rg_name               = azurerm_resource_group.sql-server.name
  sqlsvr_name           = "${var.project}-${var.environment}-sql-primary"
  sql_version           = "12.0"
  sql_admin_username    = azurerm_key_vault_secret.sqladminusername.value
  sql_admin_password    = azurerm_key_vault_secret.sqladminpassword.value
  sqladmins             = var.sql_admin_group
  bjssvpn               = var.primary_vpn
}

resource "azurerm_sql_virtual_network_rule" "sqlvnetrule" {
  name                = "${var.project}-${var.environment}-subnet-rule"
  resource_group_name = azurerm_resource_group.sql-server.name
  server_name         = "${var.project}-${var.environment}-sql-primary"
  subnet_id           = azurerm_subnet.backend.id
  count               = var.environment != "dr" ? 1 : 0

  depends_on = [
    module.sql_server_pri
  ]
}

 module "sql_server_sec" {
  source                = "./modules/bc_sql_server"

  count                 = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0 
  
  environment           = var.environment
  region                = local.sql_region2
  project               = var.project
  rg_name               = azurerm_resource_group.sql-server.name
  sqlsvr_name           = "${var.project}-${var.environment}-sql-secondary"
  sql_version           = "12.0"
  sql_admin_username    = azurerm_key_vault_secret.sqladminusername.value
  sql_admin_password    = azurerm_key_vault_secret.sqladminpassword.value
  sqladmins             = var.sql_admin_group
  bjssvpn               = var.primary_vpn
}
