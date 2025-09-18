module "documentstorageaccount" {
  source = "./modules/storage_account"

  storage_account_name = "${var.project}${local.environment_short_name}docsa"
  environment          = var.environment
  region               = var.region

  resource_group = azurerm_resource_group.storageaccount.name
  key_vault_id   = module.keyvault[0].keyvault_id

  ip_rules   = concat(var.primary_vpn, var.secondary_vpn)
  subnet_ids = [data.azurerm_subnet.default-subnet.id, azurerm_subnet.backend.id]
}

resource "azurerm_storage_container" "order_pdf_container" {
  name                  = "orderpdfs"
  storage_account_id    = module.documentstorageaccount.storage_account_id
  container_access_type = "container"
  depends_on            = [module.documentstorageaccount]
}

resource "azurerm_storage_container" "public_documents_container" {
  name                  = "publicdocs"
  storage_account_id    = module.documentstorageaccount.storage_account_id
  container_access_type = "container"
  depends_on            = [module.documentstorageaccount]
}
