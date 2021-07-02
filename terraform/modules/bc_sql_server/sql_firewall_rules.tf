#SQL Firewall rule to allow internal Azure Services to connect to DB
resource "azurerm_sql_firewall_rule" "sql_azure_services" {
  name                = "azure_services"
  resource_group_name = var.rg_name
  server_name         = azurerm_sql_server.sql_server.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

resource "azurerm_sql_firewall_rule" "sql_bjss_vpn" {
  name                = "AllowBjssVpn"
  resource_group_name = var.rg_name
  server_name         = azurerm_sql_server.sql_server.name
  start_ip_address    = var.bjssvpn # 
  end_ip_address      = var.bjssvpn # data.azurerm_key_vault_secret.bjssvpn.value
}


resource "azurerm_sql_firewall_rule" "sql_mastek_vpn" {
  name                = "AllowMastekVpn"
  resource_group_name = var.rg_name
  server_name         = azurerm_sql_server.sql_server.name
  start_ip_address    = var.mastekvpn # data.azurerm_key_vault_secret.bjssvpn.value
  end_ip_address      = var.mastekvpn # data.azurerm_key_vault_secret.bjssvpn.value
}
