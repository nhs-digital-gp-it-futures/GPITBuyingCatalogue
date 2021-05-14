resource "azurerm_resource_group" "webapp" {
  name          = "${var.project}-${var.environment}-rg-webapp"
  location      = var.region
  tags = {
    environment = var.environment
  }
}
