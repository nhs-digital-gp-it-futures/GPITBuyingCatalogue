resource "azurerm_application_insights" "appinsights" {
  name                = "${var.project}-${var.environment}-appinsights"
  location            = var.region
  resource_group_name = azurerm_resource_group.app-insights.name
  application_type    = "web"
  
  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_application_insights_standard_web_test" "app_webtest" {
  count                   = local.core_env != "dev" ? 1 : 0
  name                    = "health-check"
  resource_group_name     = azurerm_resource_group.app-insights.name
  location                = var.region
  application_insights_id = azurerm_application_insights.appinsights.id
  geo_locations           = [
    "emea-se-sto-edge", // UK West
    "emea-ru-msa-edge", // UK South. Note: Contrary to the naming, this is not based in Russia. https://stackoverflow.com/a/57629988
    "emea-gb-db3-azr", // North Europe
    "emea-nl-ams-azr"  // West Europe
  ]

  request {
    url = "https://${var.app_url}"
  }
}

output "instrumentation_key" {
  value               = azurerm_application_insights.appinsights.instrumentation_key
  sensitive           = true
}

output "app_id" {
  value               = azurerm_application_insights.appinsights.app_id
}