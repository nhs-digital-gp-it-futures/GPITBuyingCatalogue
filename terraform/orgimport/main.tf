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

resource "azurerm_resource_group" "org_import_rg" {
  name      = "${local.project_environment}-org-rg"
  location  = var.region
}

resource "azurerm_service_plan" "org_import_plan" {
  name                = "${local.project_environment}-org-service-plan"
  resource_group_name = azurerm_resource_group.org_import_rg.name
  location            = azurerm_resource_group.org_import_rg.location
  sku_name            = "P2v3"
  os_type             = "Windows"
}

resource "azurerm_windows_web_app" "org_import_webapp" {
  name                = "${local.project_environment}-org-webapp"
  resource_group_name = azurerm_resource_group.org_import_rg.name
  location            = azurerm_service_plan.org_import_plan.location
  service_plan_id     = azurerm_service_plan.org_import_plan.id

  site_config {
    worker_count      = 1
    use_32_bit_worker = true
    ip_restriction {
      action     = "Deny"
      name       = "Block All"
      ip_address = "0.0.0.0/32"
      priority   = 200
      headers    = []
    }
  }

  app_settings = {
    DOTNET_APPLICATIONINSIGHTS__CONNECTION_STRING = data.azurerm_application_insights.app_insights.connection_string
    BUYINGCATALOGUECONNECTIONSTRING               = "Server=tcp:${data.azurerm_mssql_server.buyingcataloguedb.fully_qualified_domain_name},1433;Initial Catalog=${var.database_catalog};Persist Security Info=False;User ID=${data.azurerm_key_vault_secret.sqladminusername.value};Password=${data.azurerm_key_vault_secret.sqladminpassword.value};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

  lifecycle {
    ignore_changes = [
      app_settings
    ]
  }
}