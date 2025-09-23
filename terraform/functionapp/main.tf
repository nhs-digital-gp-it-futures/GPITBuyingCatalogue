terraform {
  required_version = ">= 1.10.1"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.13.0"
    }
  }
  backend "azurerm" {
  }
}

provider "azurerm" {
  storage_use_azuread = true
  features {
  }
}

data "azurerm_subscription" "current" {}

provider "azurerm" {
  alias           = "infrastructure"
  subscription_id = var.infrastructure_subscription_id
  features {}
}

resource "azurerm_resource_group" "function_app_rg" {
  name     = "${local.project_environment}-rg-functionapp"
  location = var.region
}

resource "azurerm_virtual_network" "function_app_vnet" {
  name                = "${local.project_environment}-fa-vnet"
  location            = var.region
  resource_group_name = azurerm_resource_group.function_app_rg.name
  address_space       = ["10.0.0.0/22"]

  tags = {
    environment = var.environment
  }
}

resource "azurerm_subnet" "function_app_subnet" {
  name                 = "default"
  virtual_network_name = azurerm_virtual_network.function_app_vnet.name
  resource_group_name  = azurerm_resource_group.function_app_rg.name
  address_prefixes     = ["10.0.0.0/24"]
  service_endpoints    = ["Microsoft.Storage"]
  
  delegation {
    name = "functionapp-delegation"

    service_delegation {
      name = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}

resource "azurerm_service_plan" "function_app_plan" {
  name                = "${local.project_environment}-functionapp-service-plan"
  location            = azurerm_resource_group.function_app_rg.location
  resource_group_name = azurerm_resource_group.function_app_rg.name
  sku_name            = "P2v2"
  os_type             = "Windows"
}

resource "azurerm_storage_account" "function_app_storage" {
  name                      = "${var.project}${local.environment_short_name}fast"
  location                  = azurerm_resource_group.function_app_rg.location
  resource_group_name       = azurerm_resource_group.function_app_rg.name
  account_tier              = "Standard"
  account_replication_type  = "LRS"
  shared_access_key_enabled = false

  network_rules {
    default_action             = "Deny"
    ip_rules                   = var.primary_vpn
    virtual_network_subnet_ids = [azurerm_subnet.function_app_subnet.id, data.azurerm_subnet.default-subnet.id]
  }
}

data "azurerm_role_definition" "storage_service_props_reader" {
  name        = local.role_definition_name
  scope       = data.azurerm_subscription.current.id
}

resource "azurerm_storage_container" "function_app_container" {
  name                  = "capabilities-update"
  storage_account_id    = azurerm_storage_account.function_app_storage.id
  container_access_type = "container"
}

resource "azurerm_storage_queue" "send_email_queue" {
  name                 = local.send_notification
  storage_account_name = azurerm_storage_account.function_app_storage.name
}

resource "azurerm_storage_queue" "complete_email_queue" {
  name                 = local.complete_notification
  storage_account_name = azurerm_storage_account.function_app_storage.name
}

resource "azurerm_user_assigned_identity" "function_identity" {
  resource_group_name = azurerm_resource_group.function_app_rg.name
  location = azurerm_resource_group.function_app_rg.location
  name = "${local.project_environment}-functionapp-managed-id"
}

resource "azurerm_role_assignment" "service_props_reader_assignment" {
  scope              = azurerm_storage_account.function_app_storage.id
  role_definition_id = data.azurerm_role_definition.storage_service_props_reader.id
  principal_id       = azurerm_user_assigned_identity.function_identity.principal_id
}

resource "azurerm_role_assignment" "blob_data_assignment" {
  scope                = azurerm_storage_account.function_app_storage.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.function_identity.principal_id
}

resource "azurerm_role_assignment" "queue_data_assignment" {
  scope                = azurerm_storage_account.function_app_storage.id
  role_definition_name = "Storage Queue Data Contributor"
  principal_id         = azurerm_user_assigned_identity.function_identity.principal_id
}

resource "azurerm_role_assignment" "table_data_assignment" {
  scope                = azurerm_storage_account.function_app_storage.id
  role_definition_name = "Storage Table Data Contributor"
  principal_id         = azurerm_user_assigned_identity.function_identity.principal_id
}

resource "azurerm_windows_function_app" "function_app" {
  name                      = "${local.project_environment}-functionapp"
  virtual_network_subnet_id = azurerm_subnet.function_app_subnet.id

  app_settings = {
    AZURE_CLIENT_ID                       = azurerm_user_assigned_identity.function_identity.client_id
    AzureWebJobsStorage__accountName      = azurerm_storage_account.function_app_storage.name
    AzureWebJobsStorage__credential       = "managedidentity"
    AzureWebJobsStorage__clientId         = azurerm_user_assigned_identity.function_identity.client_id

    APPLICATIONINSIGHTS_CONNECTION_STRING = data.azurerm_application_insights.app_insights.connection_string
    BUYINGCATALOGUECONNECTIONSTRING       = "Server=tcp:${data.azurerm_mssql_server.buyingcataloguedb.fully_qualified_domain_name},1433;Initial Catalog=${var.database_catalog};Persist Security Info=False;User ID=${data.azurerm_key_vault_secret.sqladminusername.value};Password=${data.azurerm_key_vault_secret.sqladminpassword.value};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    NOTIFY_API_KEY                        = var.notify_api_key
    QUEUE__SEND_EMAIL_NOTIFICATION        = local.send_notification
    QUEUE__COMPLETE_EMAIL_NOTIFICATION    = local.complete_notification
    TRUDAPI__APIKEY                       = var.trud_api_key
    TRUDAPI__ITEMID                       = var.trud_item_id
    OrganisationUri                       = "https://directory.spineservices.nhs.uk/ORD/2-0-0/organisations"
    RelationshipsUri                      = "https://directory.spineservices.nhs.uk/ORD/2-0-0/rels"
    RolesUri                              = "https://directory.spineservices.nhs.uk/ORD/2-0-0/roles"
    SearchUri                             = "https://directory.spineservices.nhs.uk/ORD/2-0-0/sync"
  }

  identity {
    type = "UserAssigned"
    identity_ids = [ azurerm_user_assigned_identity.function_identity.id ]
  }

  ftp_publish_basic_authentication_enabled       = false
  webdeploy_publish_basic_authentication_enabled = false
  service_plan_id                                = azurerm_service_plan.function_app_plan.id
  location                                       = azurerm_resource_group.function_app_rg.location
  resource_group_name                            = azurerm_resource_group.function_app_rg.name
  storage_account_name                           = azurerm_storage_account.function_app_storage.name
  storage_uses_managed_identity                  = true
  https_only                                     = true
  enabled                                        = true
  public_network_access_enabled                  = true

  site_config {
    always_on                         = true
    ftps_state                        = "Disabled"
    ip_restriction_default_action     = "Deny"
    scm_ip_restriction_default_action = "Deny"
    http2_enabled                     = true
    use_32_bit_worker                 = false

    dynamic "ip_restriction" {
      for_each = var.primary_vpn

      content {
        name       = "VPN_ACCESS_${ip_restriction.key}"
        ip_address = "${ip_restriction.value}/32"
        priority   = 300 + ip_restriction.key
      }
    }
    
    ip_restriction {
      ip_address = var.nhsd_network_range
    }

    scm_ip_restriction {
      action                    = "Allow"
      virtual_network_subnet_id = data.azurerm_subnet.default-subnet.id
    }
  }

  depends_on = [
    azurerm_role_assignment.service_props_reader_assignment,
    azurerm_role_assignment.blob_data_assignment,
    azurerm_role_assignment.queue_data_assignment,
    azurerm_role_assignment.table_data_assignment
  ]
}
