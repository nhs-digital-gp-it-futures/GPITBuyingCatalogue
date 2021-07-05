resource "azurerm_app_service_plan" "webapp_sp" {
  name                = "${var.webapp_name}-service-plan"
  location            = var.region
  resource_group_name = var.rg_name
  kind                = "Linux"
  reserved            = true

  sku {
    tier = var.sku_tier
    size = var.sku_size
  }

  tags                      = {
    environment             = var.environment,
    architecture            = "new"
  }
}

resource "azurerm_app_service" "webapp" {
  name                = var.webapp_name
  location            = var.region
  resource_group_name = var.rg_name
  app_service_plan_id = azurerm_app_service_plan.webapp_sp.id
  
  app_settings = {
    # Main Settings
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = false
    ASPNETCORE_ENVIRONMENT: var.aspnet_environment

    APPINSIGHTS_INSTRUMENTATIONKEY      = var.instrumentation_key
    BC_SMTP_HOST                        = ""
    BC_SMTP_PORT                        = ""
    
    # Settings for Container Registy  
    DOCKER_REGISTRY_SERVER_URL          = "https://${data.azurerm_container_registry.acr.login_server}"
    DOCKER_REGISTRY_SERVER_USERNAME     = data.azurerm_container_registry.acr.admin_username
    DOCKER_REGISTRY_SERVER_PASSWORD     = var.acr_pwd

    # Settings for sql
    BC_DB_CONNECTION                    = "Server=tcp:${data.azurerm_sql_server.sql_server.fqdn},1433;Initial Catalog=${var.db_name_main};Persist Security Info=False;User ID=${data.azurerm_sql_server.sql_server.administrator_login};Password=${var.auth_pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"    
    CO_DB_CONNECTION                    = "todo"
    BC_BLOB_CONNECTION                  = var.sa_connection_string
    BC_BLOB_CONTAINER                   = "documents"
    
    WEBSITE_HTTPLOGGING_RETENTION_DAYS  = "2"
    
    OPERATING_MODE                      = "Private"
  }
  
  # Configure Docker Image to load on start
  site_config {
    linux_fx_version          = "DOCKER|${data.azurerm_container_registry.acr.login_server}/${var.repository_name}:latest"
    use_32_bit_worker_process = true
    always_on                 = var.always_on
    min_tls_version           = "1.2"
    ip_restriction {
      name       = "NHS_Access"
      ip_address = data.azurerm_key_vault_secret.nhsdoffice1.value
      priority   = 200
      headers    = []
    }
    ip_restriction {
      name       = "BJSS_VPN"
      ip_address = "${var.primary_vpn}/32"
      priority   = 210
      headers    = []
    }
    ip_restriction {
      name       = "Mastek_VPN"
      ip_address = "${var.secondary_vpn}/32,${var.tertiary_vpn}/32"
      priority   = 220
      headers    = []
    }
    ip_restriction {
      name       = "Devops"
      ip_address = "51.104.26.0/24"
      priority   = 230
      headers    = []
    }
    scm_use_main_ip_restriction = false
  }
  identity {
    type = "SystemAssigned"
  }
  
  tags                      = {
    environment             = var.environment,
    architecture            = "new"
  }

  lifecycle {
    ignore_changes = [
      app_settings,
      site_config[0].ip_restriction[0]
    ]
  }
}
