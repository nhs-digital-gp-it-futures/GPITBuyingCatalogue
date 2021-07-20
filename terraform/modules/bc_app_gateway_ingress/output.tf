output "appgateway_name" {
    description = "The Application Gateway Name"
    value = azurerm_application_gateway.app_gateway.name
    sensitive   = false
}

output "appgateway_pip_fqdn" {
    description = "The Application Gateway Public Name"
    value = azurerm_public_ip.pip_app_gateway.fqdn
}

output "appgateway_pip_ipaddress" {
    description = "The Application Gateway Public IP Address"
    value = azurerm_public_ip.pip_app_gateway.ip_address
}

output "appgateway_site_URL" {
    value = var.core_url
}
