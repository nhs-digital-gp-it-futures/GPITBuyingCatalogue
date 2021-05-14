terraform {
  required_version        = ">= 0.14"
  backend "azurerm" {
  }
}

provider "azurerm" {
  subscription_id = var.subscription_id
  tenant_id       = var.tenant_id

  features {
  }
}
