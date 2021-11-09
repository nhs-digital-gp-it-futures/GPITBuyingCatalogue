resource "azurerm_network_security_rule" "Public" {
  name                        = "AllowPublicAccess"
  count                       = local.shortenv == "production" ? 1 : 0 
  resource_group_name         = azurerm_resource_group.app-gateway.name
  network_security_group_name = azurerm_network_security_group.gateway.name
  source_address_prefix       = "AzureCloud"
  destination_address_prefix  = "*"
  source_port_range           = "*"
  destination_port_ranges     = [ "80", "443" ]
  direction                   = "Inbound"
  access                      = "Allow"
  protocol                    = "*"
  priority                    = 100
  description                 = "Allow public access to this environment"
}
resource "azurerm_network_security_rule" "VPN_Access" {
  name                        = "AllowBjssVpn"
  resource_group_name         = azurerm_resource_group.app-gateway.name
  network_security_group_name = azurerm_network_security_group.gateway.name
  source_address_prefix       = var.primary_vpn
  destination_address_prefix  = "*"
  source_port_range           = "*"
  destination_port_ranges     = [ "80", "443" ]
  direction                   = "Inbound"
  access                      = "Allow"
  protocol                    = "*"
  priority                    = "160"
  description                 = "Allow staff access who are connect to the BJSS VPN"
}

resource "azurerm_network_security_rule" "DevOps" {
  name                        = "AllowAzureDevOps"
  resource_group_name         = azurerm_resource_group.app-gateway.name
  network_security_group_name = azurerm_network_security_group.gateway.name
  source_address_prefix       = "AzureCloud"
  destination_address_prefix  = "*"
  source_port_range           = "*"
  destination_port_ranges     = [ "80", "443" ]
  direction                   = "Inbound"
  access                      = "Allow"
  protocol                    = "*"
  priority                    = 200
  description                 = "Allow AzureDevOps access to this environment"
}

resource "azurerm_network_security_rule" "Azure" {
  name                        = "AllowAzureInfrastructurePorts"
  resource_group_name         = azurerm_resource_group.app-gateway.name
  network_security_group_name = azurerm_network_security_group.gateway.name
  source_address_prefix       = "*"
  destination_address_prefix  = "*"
  source_port_range           = "*"
  destination_port_range      = "65200-65535"
  direction                   = "Inbound"
  access                      = "Allow"
  protocol                    = "*"
  priority                    = "500"
  description                 = "Allow incoming Azure Gateway Manager and inbound virtual network traffic (VirtualNetwork tag) on the NSG."
}
