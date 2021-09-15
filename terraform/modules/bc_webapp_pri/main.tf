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
    ASPNETCORE_ENVIRONMENT              = var.aspnet_environment

    APPINSIGHTS_INSTRUMENTATIONKEY      = var.instrumentation_key
    BC_SMTP_HOST                        = var.smtp_server_host
    BC_SMTP_PORT                        = var.smtp_server_port   

    # Settings for Container Registy  
    DOCKER_REGISTRY_SERVER_URL          = "https://${var.docker_registry_server_url}" 
    DOCKER_REGISTRY_SERVER_USERNAME     = var.docker_registry_server_username
    DOCKER_REGISTRY_SERVER_PASSWORD     = var.docker_registry_server_password
 
    DOMAIN_NAME                         = var.app_dns_url
    
    # Settings for sql
    BC_DB_CONNECTION                    = "Server=tcp:${data.azurerm_sql_server.sql_server.fqdn},1433;Initial Catalog=${var.db_name_main};Persist Security Info=False;User ID=${data.azurerm_sql_server.sql_server.administrator_login};Password=${var.auth_pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"    
    
    WEBSITE_HTTPLOGGING_RETENTION_DAYS  = "2"
  }
  
  # Configure Docker Image to load on start
  site_config {
    linux_fx_version          = "DOCKER|https://${var.docker_registry_server_url}/${var.repository_name}:latest"
    use_32_bit_worker_process = true
    always_on                 = var.always_on
    min_tls_version           = "1.2"
    ip_restriction {
      name       = "APP_GATEWAY_ACCESS"
      ip_address = "${var.app_gateway_ip}/32"
      priority   = 200
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
      site_config[0].linux_fx_version
    ]
  }
}
