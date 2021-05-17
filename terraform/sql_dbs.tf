module "sql_databases_pri" {
  source                = "./modules/bc_sql_dbs_webapp"

  environment           = var.environment
  region                = var.region
  project               = var.project
  rg_name               = "${var.project}-${var.environment}-rg-sql-pri"
  sqlsvr_name           = "${var.project}-${var.environment}-sql-pri"
  db_name               = "private-"
  sql_collation         = "SQL_Latin1_General_CP1_CI_AS"
  sql_edition           = "Standard"
  sql_size              = "S0"
} 
