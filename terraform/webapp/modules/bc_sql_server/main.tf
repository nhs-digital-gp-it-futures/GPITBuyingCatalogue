resource "azurerm_mssql_server" "sql_server" {
  name                          = var.sqlsvr_name
  resource_group_name           = var.resource_group
  location                      = var.region
  version                       = var.sql_version
  administrator_login           = var.sql_admin_username
  administrator_login_password  = var.sql_admin_password

  identity {
    type                        = "SystemAssigned"
  }
  
  azuread_administrator {
    login_username              = var.sqladmins
    object_id                   = var.sqladmins
  }

  tags = {
    environment                 = var.environment,
    architecture                = "new"
  }
}

resource "azurerm_mssql_firewall_rule" "sql_azure_services" {
  name                = "azure_services"
  server_id           = azurerm_mssql_server.sql_server.id
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

resource "azurerm_mssql_firewall_rule" "sql_bjss_vpn" {
  name                = "AllowBjssVpn"
  server_id           = azurerm_mssql_server.sql_server.id
  start_ip_address    = var.bjssvpn
  end_ip_address      = var.bjssvpn
}

resource "azurerm_mssql_server_extended_auditing_policy" "auditing" {
  server_id              = azurerm_mssql_server.sql_server.id 
  log_monitoring_enabled = true
}
