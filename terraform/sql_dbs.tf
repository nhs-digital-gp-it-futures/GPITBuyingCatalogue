module "sql_databases_pri" {
  source                = "./modules/bc_sql_dbs_webapp"

  environment           = var.environment
  region                = var.region
  project               = var.project
  rg_name               = azurerm_resource_group.sql-server.name
  server_id             = module.sql_server_pri.sql_server_id  
  sqlsvr_name           = "${var.project}-${var.environment}-sql-primary"
  db_name               = "-"
  sql_collation         = "SQL_Latin1_General_CP1_CI_AS"
  sql_edition           = "Standard"
  sql_size              = "S0"

  enable_replica        = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0 
  region_replica        = local.sql_region2
  rg_replica_name       = azurerm_resource_group.sql-server.name
  sqlsvr_replica_name   = "${var.project}-${var.environment}-sql-secondary"
  
  core_env              = var.core_env

  depends_on = [module.sql_server_pri]
} 
