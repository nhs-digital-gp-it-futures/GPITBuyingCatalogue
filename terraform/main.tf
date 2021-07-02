terraform {
  required_version = ">= 0.14"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.58.0"
    }
  }
  backend "azurerm" {
  }
}

provider "azurerm" {
  features {
  }
}
