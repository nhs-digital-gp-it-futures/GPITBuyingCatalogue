resource "azurerm_application_gateway" "app_gateway" {
  name                = local.gateway_name
  location            = var.region
  resource_group_name = var.rg_name
  count               = var.core_env != "dev" ? 1 : 0
  sku {
    name     = "WAF_v2"
    tier     = "WAF_v2"
    capacity = var.ag_capacity
  }

  custom_error_configuration {
    status_code           = "HttpStatus502"
    custom_error_page_url = "https://gpitfuturesappgwcontent.z33.web.core.windows.net/index.html"
  }

  gateway_ip_configuration {
    name      = local.gateway_ip_name
    subnet_id = var.ag_subnet_id
  }

  frontend_ip_configuration {
    name                 = local.frontend_ip_name
    public_ip_address_id = azurerm_public_ip.pip_app_gateway[0].id
  }

  backend_address_pool {
    name  = local.backend_address_pool_name
    fqdns = [var.app_service_hostname]
  }

  backend_http_settings {
    name                                = local.backend_http_settings_name
    cookie_based_affinity               = "Disabled"
    path                                = "/"
    port                                = 80
    protocol                            = "Http"
    request_timeout                     = 10
    pick_host_name_from_backend_address = true
  }

  frontend_port {
    name = local.http_frontend_port_name
    port = 80
  }

  frontend_port {
    name = local.https_frontend_port_name
    port = 443
  }

  ssl_policy {
    policy_type = "Predefined"
    policy_name = "AppGwSslPolicy20170401S"
  }

  # Static Sites

  http_listener {
    name                           = local.http_listener_name
    frontend_ip_configuration_name = local.frontend_ip_name
    frontend_port_name             = local.http_frontend_port_name
    protocol                       = "Http"
  }

  request_routing_rule {
    name                        = local.http_request_routing_name
    rule_type                   = "Basic"
    http_listener_name          = local.http_listener_name
    redirect_configuration_name = local.http_redirect_config_name
    rewrite_rule_set_name       = local.redirect_ruleset_name
    priority                    = 10010
  }

  http_listener {
    name                           = local.https_listener_name
    frontend_ip_configuration_name = local.frontend_ip_name
    frontend_port_name             = local.https_frontend_port_name
    protocol                       = "Https"
    ssl_certificate_name           = var.ssl_cert_name
  }

  request_routing_rule {
    name                        = local.https_request_routing_name
    rule_type                   = "Basic"
    http_listener_name          = local.https_listener_name
    backend_address_pool_name   = local.backend_address_pool_name
    backend_http_settings_name  = local.backend_http_settings_name
    rewrite_rule_set_name       = local.redirect_ruleset_name
    priority                    = 10020
  }

  redirect_configuration {
    name = local.http_redirect_config_name
    redirect_type = "Permanent"
    target_listener_name = local.https_listener_name
    include_path = true
    include_query_string = true
  }

  # Rewrite rules
  rewrite_rule_set {
    name = local.redirect_ruleset_name
    rewrite_rule {
      name          = "StrictTransportSecurityRule"
      rule_sequence = 1
      response_header_configuration {
        header_name  = "Strict-Transport-Security"
        header_value = "max-age=86400; includeSubDomains"
      }
    }

    rewrite_rule {
      name          = "RewriteUrlFromAzurewebsites"
      rule_sequence = 2
      condition {
        variable = "http_resp_Location"
        pattern = "(https?):.*azurewebsites.net(.*)$"                   
        ignore_case = true
      }

      response_header_configuration {
        header_name  = "Location"
        header_value = "{http_resp_Location_1}://${var.app_dns_url}{http_resp_Location_2}"
      }
    }
  }

  ssl_certificate {
    name                = var.ssl_cert_name
    key_vault_secret_id = var.ssl_cert_secret_id
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [data.azurerm_user_assigned_identity.managed_identity_aad.id]
  }

  probe {
    name                                      = local.healthprobe_name
    host                                      = var.app_service_hostname
    pick_host_name_from_backend_http_settings = false
    path                                      = "/"
    interval                                  = 30
    timeout                                   = 30
    unhealthy_threshold                       = 3
    protocol                                  = "Http"
  }

  waf_configuration {
    enabled                   = true
    firewall_mode             = "Prevention"
    rule_set_type             = "OWASP"
    rule_set_version          = "3.2"
    request_body_check        = true
    max_request_body_size_kb  = 128

    disabled_rule_group {
       rule_group_name = "REQUEST-942-APPLICATION-ATTACK-SQLI"
       rules           = [
        942380,
        942430,
        942400,
        942440,
        942450,
        942130
       ]
    }

    disabled_rule_group {
       rule_group_name = "REQUEST-920-PROTOCOL-ENFORCEMENT"
       rules           = [ 920230 ]
    }

    disabled_rule_group {
      rule_group_name = "REQUEST-931-APPLICATION-ATTACK-RFI"
      rules           = [ 931130 ]
    }

    exclusion {
      match_variable          = "RequestCookieNames"
      selector_match_operator = "Equals"
      selector                = "buyingcatalogue-cookie-consent"
    }

    exclusion {
      match_variable          = "RequestArgNames"
      selector_match_operator = "Equals"
      selector                = "__RequestVerificationToken"
    }
  }

  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  lifecycle {
    ignore_changes = [ ]
  }
}
