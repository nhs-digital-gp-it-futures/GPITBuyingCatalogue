module "sql_databases_pri" {
  source                = "./modules/bc_sql_dbs_webapp"

  environment           = var.environment
  region                = var.region
  project               = var.project
  rg_name               = azurerm_resource_group.sql-primary.name
  sqlsvr_name           = "${var.project}-${var.environment}-sql-primary"
  db_name               = "-"
  sql_collation         = "SQL_Latin1_General_CP1_CI_AS"
  sql_edition           = "Standard"
  sql_size              = "S0"

  depends_on = [module.sql_server_pri]
} 
