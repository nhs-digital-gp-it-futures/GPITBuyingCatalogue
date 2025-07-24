locals {
  # Remove punctuation in namespace
  shortenv               = replace(var.environment, "-", "")
  environment_short_name = substr(var.environment, 0, 3)
  core_env               = local.shortenv == "production" || local.shortenv == "preprod" ? local.shortenv : "dev" // Otherwise, if it's not DR, we use the environment/shortenv
  sql_region2            = "ukwest"
}
