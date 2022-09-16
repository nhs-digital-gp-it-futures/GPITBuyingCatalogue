module "sql_databases_pri" {
  source                = "./modules/bc_sql_dbs_webapp"

  count                 = !local.is_dr ? 1 : 0
  environment           = var.environment
  region                = var.region
  project               = var.project
  rg_name               = join("", module.sql_server_pri[*].sql_resource_group)
  server_id             = join("", module.sql_server_pri[*].sql_server_id)
  sqlsvr_name           = "${var.project}-${var.environment}-sql-primary"
  sql_collation         = "SQL_Latin1_General_CP1_CI_AS"
  sql_edition           = "Standard"
  sql_size              = "S0"

  enable_replica        = local.shortenv == "preprod" || local.shortenv == "production" ? 1 : 0 
  region_replica        = local.sql_region2
  rg_replica_name       = join("", module.sql_server_pri[*].sql_resource_group)
  sqlsvr_replica_name   = "${var.project}-${var.environment}-sql-secondary"
  
  core_env              = local.core_env

  depends_on = [module.sql_server_pri]
} 
