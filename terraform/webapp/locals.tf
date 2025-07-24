locals {
  shortenv               = replace(var.environment, "-", "")
  environment_short_name = substr(var.environment, 0, 3)
  core_env               = local.shortenv == "production" || local.shortenv == "preprod" ? local.shortenv : "dev"
  is_live_environment    = local.shortenv == "production" || local.shortenv == "preprod"
  use_dns_cname          = local.core_env == "dev" || local.core_env == "preprod"
  use_app_gateway        = local.is_live_environment || local.shortenv == "demo"
  gateway_public_access  = local.use_app_gateway && (local.shortenv == "production" || local.shortenv == "demo")
  sql_region2            = "ukwest"
}
