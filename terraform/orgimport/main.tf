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
  name      = "${var.project}-${var.environment}-rg-organisation-importer"
  location  = var.region
}

resource "azurerm_service_plan" "org_import_plan" {
  name                = "${var.project}-${var.environment}-service-plan"
  resource_group_name = azurerm_resource_group.org_import_rg.name
  location            = azurerm_resource_group.org_import_rg.location
  sku_name            = "P2v3"
  os_type             = "Windows"
}

resource "azurerm_windows_web_app" "org_import_webapp" {
  name                = "${var.project}-${var.environment}-web-app"
  resource_group_name = azurerm_resource_group.org_import_rg.name
  location            = azurerm_service_plan.org_import_plan.location
  service_plan_id     = azurerm_service_plan.org_import_plan.id

  site_config {
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
    CONNECTION_STRING = "TODO"
  }
}