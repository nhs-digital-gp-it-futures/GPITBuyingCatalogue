resource "azurerm_linux_web_app_slot" "slot" {
  name           = "staging"
  count          = var.create_slot
  app_service_id = azurerm_linux_web_app.webapp.id

  app_settings = {
    # Main Settings
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = false
    ASPNETCORE_ENVIRONMENT              = var.aspnet_environment

    APPINSIGHTS_INSTRUMENTATIONKEY = var.instrumentation_key

    # Settings for Container Registy  
    DOCKER_REGISTRY_SERVER_URL      = "https://${var.docker_registry_server_url}"
    DOCKER_REGISTRY_SERVER_USERNAME = var.docker_registry_server_username
    DOCKER_REGISTRY_SERVER_PASSWORD = var.docker_registry_server_password

    DOMAIN_NAME = var.app_dns_url

    # Settings for sql
    BC_DB_CONNECTION                    = "Server=tcp:${data.azurerm_mssql_server.sql_server.fully_qualified_domain_name},1433;Initial Catalog=${var.db_name_main};Persist Security Info=False;User ID=${var.sql_admin_username};Password=${var.sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"    
    AZUREBLOBSETTINGS__CONNECTIONSTRING = var.blob_storage_connection_string
    
    RECAPTCHASETTINGS__SITEKEY          = var.recaptcha_site_key
    RECAPTCHASETTINGS__SECRETKEY        = var.recaptcha_secret_key

    NOTIFY_API_KEY = var.notify_api_key

    SESSION_IDLE_TIMEOUT               = "60"
    WEBSITE_HTTPLOGGING_RETENTION_DAYS = "2"
  }

  # Configure Docker Image to load on start
  site_config {
    use_32_bit_worker   = true
    always_on           = var.always_on
    minimum_tls_version = "1.2"

    application_stack {
      docker_image     = "https://${var.docker_registry_server_url}/${var.repository_name}"
      docker_image_tag = "latest"
    }

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

    scm_use_main_ip_restriction = false
  }
  identity {
    type = "SystemAssigned"
  }

  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  lifecycle {
    ignore_changes = [
      virtual_network_subnet_id,
      site_config[0].scm_minimum_tls_version,
      site_config[0].ftps_state,
      site_config[0].application_stack[0].docker_image,
      site_config[0].application_stack[0].docker_image_tag
    ]
  }
}
