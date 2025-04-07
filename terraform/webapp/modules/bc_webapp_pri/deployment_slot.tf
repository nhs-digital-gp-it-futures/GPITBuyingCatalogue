resource "azurerm_linux_web_app_slot" "slot" {
  name                                           = "staging"
  count                                          = var.create_slot
  app_service_id                                 = azurerm_linux_web_app.webapp.id
  ftp_publish_basic_authentication_enabled       = false
  webdeploy_publish_basic_authentication_enabled = false
  virtual_network_subnet_id                      = var.backend_subnet_id

  app_settings = {
    # Main Settings
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = false
    ASPNETCORE_ENVIRONMENT              = var.aspnet_environment
    ASPNETCORE_HTTP_PORTS               = "80"

    APPINSIGHTS_INSTRUMENTATIONKEY = var.instrumentation_key

    DOMAIN_NAME = var.app_dns_url

    # Settings for sql
    BC_DB_CONNECTION                    = "Server=tcp:${data.azurerm_mssql_server.sql_server.fully_qualified_domain_name},1433;Initial Catalog=${var.db_name_main};Persist Security Info=False;Authentication=Active Directory Managed Identity;User Id=${azurerm_user_assigned_identity.web_app_identity.client_id};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    AZUREBLOBSETTINGS__CONNECTIONSTRING = var.blob_storage_connection_string

    RECAPTCHASETTINGS__SITEKEY   = var.recaptcha_site_key
    RECAPTCHASETTINGS__SECRETKEY = var.recaptcha_secret_key

    NOTIFY_API_KEY = var.notify_api_key

    SESSION_IDLE_TIMEOUT = "60"
  }

  site_config {
    use_32_bit_worker             = true
    always_on                     = var.always_on
    minimum_tls_version           = "1.2"
    ip_restriction_default_action = "Deny"
    ftps_state                    = "Disabled"
    http2_enabled                 = true

    ip_restriction {
      name       = "APP_GATEWAY_ACCESS"
      ip_address = "${var.app_gateway_ip}/32"
      priority   = 200
      headers    = []
    }

    ip_restriction {
      name       = "PRIMARY_VPN"
      ip_address = "${var.primary_vpn}/32"
      priority   = 210
      headers    = []
    }

    dynamic "ip_restriction" {
      for_each = var.secondary_vpn

      content {
        name       = "SECONDARY_VPN_ACCESS_${ip_restriction.key}"
        ip_address = "${ip_restriction.value}/32"
        priority   = 300 + ip_restriction.key
      }
    }

    scm_use_main_ip_restriction = false
  }

  identity {
    type = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.web_app_identity.id]
  }

  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  lifecycle {
    ignore_changes = [
      site_config[0].scm_minimum_tls_version,
      site_config[0].ftps_state
    ]
  }
}
