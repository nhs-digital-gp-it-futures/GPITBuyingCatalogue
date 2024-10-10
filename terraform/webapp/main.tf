terraform {
  required_version = ">= 1.3.1"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.4.0"
    }
  }
  backend "azurerm" {
  }
}

provider "azurerm" {
  features {
  }
}


provider "azurerm" {
  alias = "acr"
  subscription_id = var.acr_subscription_id
  features {
  }
}

provider "azurerm" {
  alias = "dns"
  subscription_id = var.dns_subscription_id
  features {
  }
}
