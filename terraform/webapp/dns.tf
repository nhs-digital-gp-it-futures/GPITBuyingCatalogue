locals {
  dns_resource_group_name = "${var.project}-dynamic-core-rg"
  dns_resource_name = replace(var.app_url, "${var.environment}.", "")

  cname_target = local.core_env == "dev" ? module.webapp.webapp_default_site_hostname : module.appgateway.appgateway_pip_fqdn
}

resource "azurerm_dns_cname_record" "alias" {
  count               = local.core_env == "dev" || local.environment_identifier == "preprod" ? 1 : 0
  name                = var.environment
  zone_name           = local.dns_resource_name
  resource_group_name = local.dns_resource_group_name
  ttl                 = 3600
  record              = local.cname_target

  depends_on = [module.webapp]
}

resource "azurerm_dns_txt_record" "verification" {
  count               = local.core_env == "dev" || local.environment_identifier == "preprod" ? 1 : 0
  name                = "asuid.${var.environment}"
  zone_name           = local.dns_resource_name
  resource_group_name = local.dns_resource_group_name
  ttl                 = 3600

  record {
    value = module.webapp.webapp_verification_id
  }

  depends_on = [module.webapp]
}
