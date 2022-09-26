locals {
  # Remove punctuation in namespace
  shortenv                = replace(var.environment, "-", "")
  is_dr                   = length(regexall("^dr", local.shortenv)) > 0
  environment_identifier  = local.is_dr ? var.primary_env : local.shortenv
  core_env                = local.environment_identifier == "production" || local.environment_identifier == "preprod" ? local.environment_identifier : "dev" // Otherwise, if it's not DR, we use the environment/shortenv
  gw_webappURL            = "${local.shortenv}.${var.coreurl}"
  sql_region2             = "ukwest"
}
