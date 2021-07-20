resource "azurerm_resource_group" "webapp" {
  name           = "${var.project}-${var.environment}-rg-webapp"
  location       = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "storageaccount" {
  name           = "${var.project}-${var.environment}-rg-storage"
  location       = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "keyvault" {
  name           = "${var.project}-${var.environment}-rg-keyvault"
  location       = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "sql-primary" {
  name           = "${var.project}-${var.environment}-rg-sql-primary"
  location       = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "app-insights" {
  name           = "${var.project}-${var.environment}-rg-appinsights"
  location       = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "virtualnet" {
  name           = "${var.project}-${var.environment}-rg-virtualnet"
  location       = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "app-gateway" {
  name           = "${var.project}-${var.environment}-rg-appgateway"
  location       = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}