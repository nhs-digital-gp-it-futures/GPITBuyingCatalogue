module "keyvault" {
  source                    = "./modules/keyvault"

  count = !local.is_dr ? 1 : 0

  environment               = var.environment
  region                    = var.region
  tenant_id                 = var.tenant_id
  principal_id              = azurerm_user_assigned_identity.managed_id.principal_id
  project                   = var.project
  pjtcode                   = var.pjtcode
  core_env                  = local.core_env
  keyvault_core_id          = data.azurerm_key_vault.keyvault_core.id
  kv_sqlusername            = var.kv_sqlusername
  kv_access_group           = var.kv_access_group
}