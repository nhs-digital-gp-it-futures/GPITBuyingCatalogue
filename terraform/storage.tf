module "storage_account" {
  source               = "./modules/bc_storage_account"
  environment          = var.environment
  region               = var.region
  project              = var.project
  
  rg_name              = azurerm_resource_group.storageaccount.name
  storage_account_name = "${var.project}${var.environment}sa"
  replication_type     = "grs"
  container_name       = "documents"
  ip_rules             = [""]
  vnet_subnet_id       = ""
  kv_id                = local.kv_id
  kv_key               = "${var.project}${var.environment}-storage-constring"
}
