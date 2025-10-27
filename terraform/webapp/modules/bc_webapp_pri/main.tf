resource "azurerm_service_plan" "webapp_sp" {
  name                = "${var.webapp_name}-service-plan"
  location            = var.region
  resource_group_name = var.rg_name
  os_type             = "Linux"
  sku_name            = var.sku_size

  tags = {
    environment  = var.environment,
    architecture = "new"
  }
}

resource "azurerm_role_assignment" "example" {
  principal_id                     = var.webapp_identity.principal_id
  role_definition_name             = "AcrPull"
  scope                            = var.docker_registry_id
}

resource "azurerm_linux_web_app" "webapp" {
  name                                           = var.webapp_name
  location                                       = var.region
  resource_group_name                            = var.rg_name
  service_plan_id                                = azurerm_service_plan.webapp_sp.id
  ftp_publish_basic_authentication_enabled       = false
  webdeploy_publish_basic_authentication_enabled = false
  virtual_network_subnet_id                      = var.backend_subnet_id

  app_settings = {
    # Main Settings
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = false
    ASPNETCORE_ENVIRONMENT              = var.aspnet_environment
    ASPNETCORE_HTTP_PORTS               = "80"
    AzureBlobSettings__accountName      = var.storage_account_name
    AzureBlobSettings__clientId         = var.webapp_identity.client_id

    APPINSIGHTS_INSTRUMENTATIONKEY = var.instrumentation_key

    DOMAIN_NAME = var.app_dns_url

    # Settings for sql
    BC_DB_CONNECTION                    = "Server=tcp:${data.azurerm_mssql_server.sql_server.fully_qualified_domain_name},1433;Initial Catalog=${var.db_name_main};Persist Security Info=False;Authentication=Active Directory Managed Identity;User Id=${var.webapp_identity.client_id};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

    RECAPTCHASETTINGS__SITEKEY   = var.recaptcha_site_key
    RECAPTCHASETTINGS__SECRETKEY = var.recaptcha_secret_key

    NOTIFY_API_KEY = var.notify_api_key

    SESSION_IDLE_TIMEOUT = "60"
  }

  # Configure Docker Image to load on start
  site_config {
    use_32_bit_worker             = true
    always_on                     = var.always_on
    minimum_tls_version           = "1.2"
    ip_restriction_default_action = "Deny"
    ftps_state                    = "Disabled"
    http2_enabled                 = true
    container_registry_managed_identity_client_id = var.webapp_identity.client_id
    container_registry_use_managed_identity = true

    application_stack {
      docker_image_name        = "${var.repository_name}:latest"
      docker_registry_url      = "https://${var.docker_registry_server_url}"
    }

    dynamic "ip_restriction" {
      for_each = var.app_gateway_ip == null ? [] : tolist([var.app_gateway_ip])
      content {
        name       = "APP_GATEWAY_ACCESS"
        ip_address = "${var.app_gateway_ip}/32"
        priority   = 200
        headers    = []
      }
    }

    dynamic "ip_restriction" {
      for_each = var.vpn

      content {
        name       = "VPN_ACCESS_${ip_restriction.key}"
        ip_address = "${ip_restriction.value}/32"
        priority   = 300 + ip_restriction.key
      }
    }

    scm_use_main_ip_restriction = false
  }

  identity {
    type = "UserAssigned"
    identity_ids = [var.webapp_identity.id]
  }

  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  lifecycle {
    ignore_changes = [
      site_config[0].scm_minimum_tls_version,
      site_config[0].ftps_state,
      site_config[0].application_stack[0].docker_image_name,
    ]
  }
}
