resource "azurerm_virtual_network" "virtualnet" {
  name                = "${var.project}-${var.environment}-virtualnet"
  location            = var.region
  address_space       = ["10.1.0.0/16"]
  resource_group_name = azurerm_resource_group.virtualnet.name

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_subnet" "gateway" {
  name                              = "${var.project}-${var.environment}-gateway-subnet"
  private_endpoint_network_policies = "Enabled"
  resource_group_name               = azurerm_resource_group.virtualnet.name
  virtual_network_name              = azurerm_virtual_network.virtualnet.name
  address_prefixes = ["10.1.1.0/24"]
}

resource "azurerm_subnet" "backend" {
  name                              = "${var.project}-${var.environment}-backend-subnet"
  private_endpoint_network_policies = "Enabled"
  resource_group_name               = azurerm_resource_group.virtualnet.name
  virtual_network_name              = azurerm_virtual_network.virtualnet.name
  address_prefixes = ["10.1.2.0/24"]

  delegation {
    name = "${var.project}-${var.environment}-backend-delegation"

    service_delegation {
      name = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }

  service_endpoints = ["Microsoft.Sql", "Microsoft.Storage"]
}

resource "azurerm_app_service_virtual_network_swift_connection" "webapp" {
  app_service_id = module.webapp.webapp_service_id
  subnet_id      = azurerm_subnet.backend.id
}
