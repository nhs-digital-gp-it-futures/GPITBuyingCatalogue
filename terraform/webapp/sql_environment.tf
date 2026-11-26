module "sql_server_pri" {
  source                     = "./modules/bc_sql_server"
  count                      = 1
  environment                = var.environment
  region                     = var.region
  resource_group             = azurerm_resource_group.sql-server.name
  project                    = var.project
  sqlsvr_name                = "${var.project}-${var.environment}-sql-primary"
  sql_version                = "12.0"
  sql_admin_username         = module.keyvault[0].sqladminusername
  sql_admin_password         = module.keyvault[0].sqladminpassword
  sqladmins                  = var.sql_admin_group
  primary_vpn                = var.primary_vpn
}

resource "azurerm_monitor_diagnostic_setting" "sql_primary_master" {
  name                       = "${var.project}-${var.environment}-sql-primary-master-diag"
  target_resource_id         = "${module.sql_server_pri[0].sql_server_id}/databases/master"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.log_analytics.id

  enabled_log {
    category = "DevOpsOperationsAudit"
  }

  enabled_log {
    category = "SQLSecurityAuditEvents"
  }
}

resource "azurerm_mssql_virtual_network_rule" "sqlvnetrule" {
  name                = "${var.project}-${var.environment}-subnet-rule"
  server_id           = join("", module.sql_server_pri[*].sql_server_id)
  subnet_id           = azurerm_subnet.backend.id
  count               = 1

  depends_on = [
    module.sql_server_pri
  ]
}

module "sql_server_sec" {
  source                     = "./modules/bc_sql_server"
  count                      = local.is_live_environment ? 1 : 0 
  environment                = var.environment
  region                     = local.sql_region2
  resource_group             = azurerm_resource_group.sql-server.name
  project                    = var.project
  sqlsvr_name                = "${var.project}-${var.environment}-sql-secondary"
  sql_version                = "12.0"
  sql_admin_username         = module.keyvault[0].sqladminusername
  sql_admin_password         = module.keyvault[0].sqladminpassword
  sqladmins                  = var.sql_admin_group
  primary_vpn                = var.primary_vpn
}

resource "azurerm_monitor_diagnostic_setting" "sql_secondary_master" {
  count = local.is_live_environment ? 1 : 0

  name                       = "${var.project}-${var.environment}-sql-secondary-master-diag"
  target_resource_id         = "${module.sql_server_sec[count.index].sql_server_id}/databases/master"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.log_analytics.id

  enabled_log {
    category = "DevOpsOperationsAudit"
  }

  enabled_log {
    category = "SQLSecurityAuditEvents"
  }
}