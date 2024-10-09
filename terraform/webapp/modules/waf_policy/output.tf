output "firewall_policy_id" {
  value = azurerm_web_application_firewall_policy.waf_policy.id
  description = "The Firewall Policy ID"
}
