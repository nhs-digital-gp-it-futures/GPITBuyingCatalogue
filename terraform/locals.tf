locals {
  # Remove punctuation in namespace
  shortenv = replace(var.environment, "-", "")
  # Find Shared components (e.g. key vaults)
  liveEnv = local.shortenv == "production" ? "prod" : "test"
  coreEnv = local.shortenv != "preprod" && local.shortenv != "production" ? "dev" : local.liveEnv
  # Secret name for keys
  secretPrefix = local.shortenv != "preprod" && local.shortenv != "production" ? "${var.pjtcode}${local.shortenv}" : "${var.pjtcode}${local.coreEnv}"
  # KeyVault IDs
  kv_id = "/subscriptions/${var.subscription_id}/resourceGroups/${azurerm_resource_group.keyvault.name}/providers/Microsoft.KeyVault/vaults/${azurerm_key_vault.keyvault.name}"
  corekv_id = "/subscriptions/${var.subscription_id}/resourceGroups/${var.project}-${local.coreEnv}-rg-kv/providers/Microsoft.KeyVault/vaults/${var.project}-${local.coreEnv}-core-kv"
  # WebApp URLs
  gw_webappURL = var.environment != "preprod" && var.environment != "prod" ? join("", ["private", trim(var.coreurl, var.environment)]) : "private.${var.coreurl}"
  aspnetcoreEnvironment = local.shortenv != "production" ? "Development" : "Production"
}
