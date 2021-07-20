module "storage_account" {
  source               = "./modules/bc_storage_account"
  environment          = var.environment
  region               = var.region
  project              = var.project
  
  rg_name              = azurerm_resource_group.storageaccount.name
  storage_account_name = "${var.project}${var.environment}sa"
  replication_type     = "grs"
  container_name       = "documents"

  vnet_subnet_id       = azurerm_subnet.backend.id
  kv_id                = local.kv_id
  kv_key               = "${var.project}${var.environment}-storage-constring"

  ip_rules              = [
    var.primary_vpn,
    var.secondary_vpn,
    var.tertiary_vpn,
    var.cicd_range
  ]

  depends_on = [azurerm_key_vault_access_policy.keyvault_pipeline_access]
}
