resource "azurerm_resource_group" "sql-server" {
  name           = "${var.project}-${var.environment}-rg-sql-server"
  location       = var.region
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_mssql_server" "sql_server" {
  name                          = var.sqlsvr_name
  resource_group_name           = azurerm_resource_group.sql-server.name
  location                      = var.region
  version                       = var.sql_version
  administrator_login           = var.sql_admin_username
  administrator_login_password  = var.sql_admin_password
  public_network_access_enabled = false
  
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
  server_id         = azurerm_mssql_server.sql_server.id
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

resource "azurerm_mssql_firewall_rule" "sql_bjss_vpn" {
  name                = "AllowBjssVpn"
  server_id         = azurerm_mssql_server.sql_server.id
  start_ip_address    = var.bjssvpn
  end_ip_address      = var.bjssvpn
}

resource "azurerm_mssql_virtual_network_rule" "sqlvnetrule" {
  name                  = "${var.project}-${var.environment}-subnet-rule"
  server_id             = azurerm_mssql_server.sql_server.id
  subnet_id             = var.subnet_backend_id
}
