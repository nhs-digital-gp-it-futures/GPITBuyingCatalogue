resource "azurerm_user_assigned_identity" "managed_id" {
  name                = "${var.project}-${var.environment}-managed-id"
  location            = var.region
  resource_group_name = azurerm_resource_group.webapp.name

  tags = {
    environment       = var.environment,
    architecture      = "new"
  }
}

resource "azurerm_user_assigned_identity" "managed_webapp_id" {
  name                = "${var.project}-${var.environment}-managed-webapp-id"
  location            = var.region
  resource_group_name = azurerm_resource_group.webapp.name

  tags = {
    environment       = var.environment,
    architecture      = "new"
  }
}
