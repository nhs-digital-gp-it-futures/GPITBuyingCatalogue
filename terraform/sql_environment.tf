module "sql_server_pri" {
  source                = "./modules/bc_sql_server"
  
  count                 = !local.is_dr ? 1 : 0
  environment           = var.environment
  region                = var.region
  project               = var.project
  sqlsvr_name           = "${var.project}-${var.environment}-sql-primary"
  sql_version           = "12.0"
  sql_admin_username    = module.keyvault[0].sqladminusername
  sql_admin_password    = module.keyvault[0].sqladminpassword
  sqladmins             = var.sql_admin_group
  bjssvpn               = var.primary_vpn
  subnet_backend_id     = azurerm_subnet.backend.id
}

module "sql_server_sec" {
  source                = "./modules/bc_sql_server"

  count                 = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0 
  
  environment           = var.environment
  region                = local.sql_region2
  project               = var.project
  sqlsvr_name           = "${var.project}-${var.environment}-sql-secondary"
  sql_version           = "12.0"
  sql_admin_username    = module.keyvault[0].sqladminusername
  sql_admin_password    = module.keyvault[0].sqladminpassword
  sqladmins             = var.sql_admin_group
  bjssvpn               = var.primary_vpn
}
