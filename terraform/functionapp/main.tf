terraform {
  required_version = ">= 1.3.1"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.18.0"
    }
  }
  backend "azurerm" {
  }
}

provider "azurerm" {
  features {
  }
}

resource "azurerm_resource_group" "function_app_rg" {
  name     = "${local.project_environment}-fa-rg"
  location = var.region
}

resource "azurerm_service_plan" "function_app_plan" {
  name                = "${local.project_environment}-fa-plan"
  location            = azurerm_resource_group.function_app_rg.location
  resource_group_name = azurerm_resource_group.function_app_rg.name
  sku_name            = "P1v2"
  os_type             = "Windows"
}

resource "azurerm_storage_account" "function_app_storage" {
  name                     = "${var.project}${local.environment_short_name}fast"
  location                 = azurerm_resource_group.function_app_rg.location
  resource_group_name      = azurerm_resource_group.function_app_rg.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_windows_function_app" "function_app" {
  name = "${local.project_environment}-fa"

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY  = data.azurerm_application_insights.app_insights.instrumentation_key
    BUYINGCATALOGUECONNECTIONSTRING = "Server=tcp:${data.azurerm_mssql_server.buyingcataloguedb.fully_qualified_domain_name},1433;Initial Catalog=${var.database_catalog};Persist Security Info=False;User ID=${data.azurerm_key_vault_secret.sqladminusername.value};Password=${data.azurerm_key_vault_secret.sqladminpassword.value};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    OrganisationUri                 = "https://directory.spineservices.nhs.uk/ORD/2-0-0/organisations"
    RelationshipsUri                = "https://directory.spineservices.nhs.uk/ORD/2-0-0/rels"
    RolesUri                        = "https://directory.spineservices.nhs.uk/ORD/2-0-0/roles"
    SearchUri                       = "https://directory.spineservices.nhs.uk/ORD/2-0-0/sync"
  }

  identity {
    type = "SystemAssigned"
  }

  service_plan_id            = azurerm_service_plan.function_app_plan.id
  location                   = azurerm_resource_group.function_app_rg.location
  resource_group_name        = azurerm_resource_group.function_app_rg.name
  storage_account_name       = azurerm_storage_account.function_app_storage.name
  storage_account_access_key = azurerm_storage_account.function_app_storage.primary_access_key
  https_only                 = true
  enabled                    = true

  site_config {
    always_on     = true
    ftps_state    = "Disabled"
    http2_enabled = true
  }

  lifecycle {
    ignore_changes = [
      app_settings
    ]
  }
}
