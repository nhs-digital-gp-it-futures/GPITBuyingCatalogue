output "sqladminusername" {
  description = "SQL Primary Username"
  value = azurerm_key_vault_secret.sqladminusername.value
}

output "sqladminpassword" {
  description = "SQL Primary Password"
  value = azurerm_key_vault_secret.sqladminpassword.value
  sensitive = true
}

output "sqlhangfireusername" {
  description = "SQL Hangfire Username"
  value = azurerm_key_vault_secret.sqlhangfireusername.value
}

output "sqlhangfirepassword" {
  description = "SQL Hangfire Password"
  value = azurerm_key_vault_secret.sqlhangfirepassword.value
  sensitive = true
}