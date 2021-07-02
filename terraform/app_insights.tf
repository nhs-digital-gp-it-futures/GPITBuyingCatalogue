resource "azurerm_application_insights" "appinsights" {
  name                = "${var.project}-${var.environment}-appinsights"
  location            = var.region
  resource_group_name = azurerm_resource_group.app-insights.name
  application_type    = "web"
}

output "instrumentation_key" {
  value               = azurerm_application_insights.appinsights.instrumentation_key
  sensitive           = true
}

output "app_id" {
  value               = azurerm_application_insights.appinsights.app_id
}