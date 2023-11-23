module "sql_server_pri" {
  source                = "./modules/bc_sql_server"
  
  count                 = !local.is_dr ? 1 : 0
  environment           = var.environment
  region                = var.region
  resource_group        = azurerm_resource_group.sql-server.name
  project               = var.project
  sqlsvr_name           = "${var.project}-${var.environment}-sql-primary"
  sql_version           = "12.0"
  sql_admin_username    = module.keyvault[0].sqladminusername
  sql_admin_password    = module.keyvault[0].sqladminpassword
  sqladmins             = var.sql_admin_group
  bjssvpn               = var.primary_vpn
  identity              = azurerm_user_assigned_identity.managed_webapp_id.id
}

resource "azurerm_mssql_virtual_network_rule" "sqlvnetrule" {
  name                = "${var.project}-${var.environment}-subnet-rule"
  server_id           = join("", module.sql_server_pri[*].sql_server_id)
  subnet_id           = azurerm_subnet.backend.id
  count               = !local.is_dr ? 1 : 0

  depends_on = [
    module.sql_server_pri
  ]
}

module "sql_server_sec" {
  source                = "./modules/bc_sql_server"

  count                 = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0 
  
  environment           = var.environment
  region                = local.sql_region2
  resource_group        = azurerm_resource_group.sql-server.name
  project               = var.project
  sqlsvr_name           = "${var.project}-${var.environment}-sql-secondary"
  sql_version           = "12.0"
  sql_admin_username    = module.keyvault[0].sqladminusername
  sql_admin_password    = module.keyvault[0].sqladminpassword
  sqladmins             = var.sql_admin_group
  bjssvpn               = var.primary_vpn
  identity              = azurerm_user_assigned_identity.managed_webapp_id.id
}
