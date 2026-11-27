resource "azurerm_resource_group" "webapp" {
  name     = "${var.project}-${var.environment}-rg-webapp"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_management_lock" "webapp_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-rg-webapp-delete-protection"
  scope      = azurerm_resource_group.webapp.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the webapp resource group and its resources."
}

resource "azurerm_resource_group" "sql-server" {
  name     = "${var.project}-${var.environment}-rg-sql-server"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_management_lock" "sql_server_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-rg-sql-server-delete-protection"
  scope      = azurerm_resource_group.sql-server.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the sql server resource group and its resources."
}

resource "azurerm_resource_group" "storageaccount" {
  name     = "${var.project}-${var.environment}-rg-storage"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_management_lock" "storageaccount_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-rg-storage-delete-protection"
  scope      = azurerm_resource_group.storageaccount.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the storageaccount resource group and its resources."
}

resource "azurerm_resource_group" "app-insights" {
  name     = "${var.project}-${var.environment}-rg-appinsights"
  location = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_management_lock" "app_insights_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-rg-appinsights-delete-protection"
  scope      = azurerm_resource_group.app-insights.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the app insights resource group and its resources."
}

resource "azurerm_resource_group" "virtualnet" {
  name     = "${var.project}-${var.environment}-rg-virtualnet"
  location = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_management_lock" "virtualnet_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-rg-virtualnet-delete-protection"
  scope      = azurerm_resource_group.virtualnet.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the virtualnet resource group and its resources."
}

resource "azurerm_resource_group" "app-gateway" {
  name     = "${var.project}-${var.environment}-rg-appgateway"
  location = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_management_lock" "app_gateway_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-rg-appgateway-delete-protection"
  scope      = azurerm_resource_group.app-gateway.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the app gateway resource group and its resources."
}

resource "azurerm_resource_group" "log-analytics" {
  name     = "${var.project}-${var.environment}-rg-log-analytics"
  location = var.region

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_management_lock" "log_analytics_rg_delete_lock" {
  name       = "${var.project}-${var.environment}-rg-log-analytics-delete-protection"
  scope      = azurerm_resource_group.log-analytics.id
  lock_level = "CanNotDelete"
  notes      = "Prevents accidental deletion of the log analytics resource group and its resources."
}
