module "sql_databases_pri" {
  source                = "./modules/bc_sql_dbs_webapp"

  count                 = !local.is_dr ? 1 : 0
  environment           = var.environment
  project               = var.project
  server_id             = join("", module.sql_server_pri[*].sql_server_id)
  sql_collation         = "SQL_Latin1_General_CP1_CI_AS"

  enable_replica        = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0 
  rg_replica_name       = azurerm_resource_group.sql-server.name
  sqlsvr_replica_name   = "${var.project}-${var.environment}-sql-secondary"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.log_analytics.id
  
  core_env              = local.core_env

  depends_on = [module.sql_server_pri]
} 
