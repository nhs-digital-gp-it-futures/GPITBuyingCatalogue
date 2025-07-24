output "appgateway_name" {
    description = "The Application Gateway Name"
    value = var.use_app_gateway ? azurerm_application_gateway.app_gateway[0].name : null
    sensitive   = false
}

output "appgateway_pip_fqdn" {
    description = "The Application Gateway Public Name"
    value = var.use_app_gateway ? azurerm_public_ip.pip_app_gateway[0].fqdn : null
}

output "appgateway_pip_ipaddress" {
    description = "The Application Gateway Public IP Address"
    value = var.use_app_gateway ? azurerm_public_ip.pip_app_gateway[0].ip_address : null
}
