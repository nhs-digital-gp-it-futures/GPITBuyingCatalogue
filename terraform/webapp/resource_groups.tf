resource "azurerm_resource_group" "webapp" {
  name     = "${var.project}-${var.environment}-rg-webapp"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "sql-server" {
  name     = "${var.project}-${var.environment}-rg-sql-server"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "storageaccount" {
  name     = "${var.project}-${var.environment}-rg-storage"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "app-insights" {
  name     = "${var.project}-${var.environment}-rg-appinsights"
  location = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "virtualnet" {
  name     = "${var.project}-${var.environment}-rg-virtualnet"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "app-gateway" {
  name     = "${var.project}-${var.environment}-rg-appgateway"
  location = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_resource_group" "log-analytics" {
  name     = "${var.project}-${var.environment}-rg-log-analytics"
  location = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}
