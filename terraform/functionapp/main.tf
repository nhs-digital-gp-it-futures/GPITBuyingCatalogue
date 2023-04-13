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
  name     = "${local.project_environment}-rg-functionapp"
  location = var.region
}

resource "azurerm_service_plan" "function_app_plan" {
  name                = "${local.project_environment}-functionapp-service-plan"
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

resource "azurerm_storage_container" "function_app_container" {
  name                  = "capabilities-update"
  storage_account_name  = azurerm_storage_account.function_app_storage.name
  container_access_type = "container"
}

resource "azurerm_windows_function_app" "function_app" {
  name = "${local.project_environment}-functionapp"

  app_settings = {
    APPLICATIONINSIGHTS_CONNECTION_STRING   = data.azurerm_application_insights.app_insights.connection_string
    BUYINGCATALOGUECONNECTIONSTRING         = "Server=tcp:${data.azurerm_mssql_server.buyingcataloguedb.fully_qualified_domain_name},1433;Initial Catalog=${var.database_catalog};Persist Security Info=False;User ID=${data.azurerm_key_vault_secret.sqladminusername.value};Password=${data.azurerm_key_vault_secret.sqladminpassword.value};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    OrganisationUri                         = "https://directory.spineservices.nhs.uk/ORD/2-0-0/organisations"
    RelationshipsUri                        = "https://directory.spineservices.nhs.uk/ORD/2-0-0/rels"
    RolesUri                                = "https://directory.spineservices.nhs.uk/ORD/2-0-0/roles"
    SearchUri                               = "https://directory.spineservices.nhs.uk/ORD/2-0-0/sync"
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

    ip_restriction {
      ip_address = var.primary_vpn
    }

    ip_restriction {
      ip_address = var.nhsd_network_range
    }
  }
}
