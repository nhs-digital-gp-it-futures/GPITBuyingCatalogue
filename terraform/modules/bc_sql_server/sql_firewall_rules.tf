#SQL Firewall rule to allow internal Azure Services to connect to DB
resource "azurerm_sql_firewall_rule" "sql_azure_services" {
  name                = "azure_services"
  count               = var.environment != "dr" ? 1 : 0
  resource_group_name = var.rg_name
  server_name         = azurerm_sql_server.sql_server[0].name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

resource "azurerm_sql_firewall_rule" "sql_bjss_vpn" {
  name                = "AllowBjssVpn"
  count               = var.environment != "dr" ? 1 : 0
  resource_group_name = var.rg_name
  server_name         = azurerm_sql_server.sql_server[0].name
  start_ip_address    = var.bjssvpn
  end_ip_address      = var.bjssvpn
}
