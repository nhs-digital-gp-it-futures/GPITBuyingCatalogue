output "webapp_name" {
    description = "The new Web App name"
    value = azurerm_app_service.webapp.name
    sensitive   = false
}

output "webapp_verification_id" {
    description = "The new Web App DNS Verification ID"
    value = azurerm_app_service.webapp.custom_domain_verification_id
    sensitive   = true
}

output "webapp_outbound_ip_addresses" {
    description = "The new Web App IP Addresses"
    value = azurerm_app_service.webapp.outbound_ip_address_list
    sensitive   = false
}

output "webapp_default_site_hostname" {
    description = "The new Web App URL"
    value = azurerm_app_service.webapp.default_site_hostname
    sensitive   = false
}
