module "documentstorageaccount" {
  source = "./modules/storage_account"

  storage_account_name = "${var.project}${local.environment_short_name}docsa"
  environment          = var.environment
  region               = var.region

  resource_group      = azurerm_resource_group.storageaccount.name
  key_vault_id        = module.keyvault[0].keyvault_id
}

resource "azurerm_storage_container" "order_pdf_container" {
  name                  = "orderpdfs"
  storage_account_name  = module.documentstorageaccount.storage_account_name
  container_access_type = "container"
  depends_on            = [module.documentstorageaccount]
}

resource "azurerm_storage_container" "public_documents_container" {
  name                  = "publicdocs"
  storage_account_name  = module.documentstorageaccount.storage_account_name
  container_access_type = "container"
  depends_on            = [module.documentstorageaccount]
}

resource "azurerm_storage_queue" "send_email_queue" {
  name                  = "send-email-notification"
  storage_account_name  = module.documentstorageaccount.storage_account_name
  depends_on            = [module.documentstorageaccount]
}

resource "azurerm_storage_queue" "complete_email_queue" {
  name                  = "complete-email-notification"
  storage_account_name  = module.documentstorageaccount.storage_account_name
  depends_on            = [module.documentstorageaccount]
}
