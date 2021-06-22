resource "azurerm_resource_group" "webapp" {
  name          = "${var.project}-${var.environment}-rg-webapp"
  location      = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "storageaccount" {
  name          = "${var.project}-${var.environment}-rg-sa"
  location      = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}
