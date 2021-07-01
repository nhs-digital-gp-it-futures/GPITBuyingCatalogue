resource "azurerm_resource_group" "webapp" {
  name          = "${var.project}-${var.environment}-rg-webapp"
  location      = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "storageaccount" {
  name          = "${var.project}-${var.environment}-rg-storage"
  location      = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "keyvault" {
  name     = "${var.project}-${var.environment}-rg-keyvault"
  location = var.region
  tags = {
    environment = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "sql-primary" {
  name     = "${var.project}-${var.environment}-rg-sql-primary"
  location = var.region
  tags = {
    environment = var.environment,
    architecture = "new"
  }
}