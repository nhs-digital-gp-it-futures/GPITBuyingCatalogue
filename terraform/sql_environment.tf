module "sql_server_pri" {
  source                = "./modules/bc_sql_server"
  
  environment           = var.environment
  region                = var.region
  project               = var.project
  rg_name               = azurerm_resource_group.sql-primary.name
  sqlsvr_name           = "${var.project}-${var.environment}-sql-primary"
  sql_version           = "12.0"
  sql_admin_username    = azurerm_key_vault_secret.sqladminusername.value
  sql_admin_password    = azurerm_key_vault_secret.sqladminpassword.value
  sqladmins             = data.azurerm_key_vault_secret.sqladmins.value
  bjssvpn               = var.primary_vpn
  mastekvpn             = var.secondary_vpn
}

# SQL Firewall rule to allow subnet access from aks network 
# Note cannot be in module due to conditional syntax on creation
# resource "azurerm_sql_virtual_network_rule" "sql_aks_net" {
#  name                = "${var.project}-${var.environment}-aks-subnet-rule"
#  resource_group_name = azurerm_resource_group.sql-pri.name
#  subnet_id           = azurerm_subnet.aks.id
#  server_name         = "${var.project}-${var.environment}-sql-pri"
#
#  depends_on = [
#      module.sql_server_pri
#  ]
#}

# module "sql_server_sec" {
#  source                = "./modules/bc_sql_server"
#
#  count                 = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0 
#  
#  environment           = var.environment
#  region                = local.sql_region2
#  project               = var.project
#  rg_name               = azurerm_resource_group.sql-sec[0].name
#  sqlsvr_name           = "${var.project}-${var.environment}-sql-sec"
#  sql_version           = "12.0"
#  sql_admin_username    = data.azurerm_key_vault_secret.sqladminusername.value
#  sql_admin_password    = data.azurerm_key_vault_secret.sqladminpassword.value
#  sqladmins             = data.azurerm_key_vault_secret.sqladmins.value
#  bjssvpn               = data.azurerm_key_vault_secret.bjssvpn.value
#  mastekvpn             = data.azurerm_key_vault_secret.mastekvpn1.value
#}
