resource "azurerm_application_gateway" "app_gateway" {
  name                = var.ag_name
  location            = var.region
  resource_group_name = var.rg_name
  count               = var.core_env != "dev" ? 1 : 0
  sku {
    name     = "Standard_v2"
    tier     = "Standard_v2"
    capacity = var.ag_capacity
  }

  custom_error_configuration {
    status_code           = "HttpStatus502"
    custom_error_page_url = "https://gpitfuturesappgwcontent.z33.web.core.windows.net/index.html"
  }

  gateway_ip_configuration {
    name      = "${var.ag_name_fragment}-gwip"
    subnet_id = var.ag_subnet_id
  }

  frontend_ip_configuration {
    name                 = "${var.ag_name_fragment}-appgateway-feip"
    public_ip_address_id = azurerm_public_ip.pip_app_gateway[0].id
  }

  backend_address_pool {
    name  = "${var.ag_name_fragment}-appgateway-beap"
    fqdns = [var.app_service_hostname]
  }

  backend_http_settings {
    name                                = "${var.ag_name_fragment}-appgateway-be-htst"
    cookie_based_affinity               = "Disabled"
    path                                = "/"
    port                                = 80
    protocol                            = "Http"
    request_timeout                     = 10
    pick_host_name_from_backend_address = true
  }

  frontend_port {
    name = "${var.ag_name_fragment}-appgateway-feport"
    port = 80
  }

  frontend_port {
    name = "${var.ag_name_fragment}-appgateway-feporthttps"
    port = 443
  }

  ssl_policy {
    policy_type = "Predefined"
    policy_name = "AppGwSslPolicy20170401S"
  }

  # Static Sites

  http_listener {
    name                           = "${var.ag_name_fragment}-appgateway-httplstn"
    frontend_ip_configuration_name = "${var.ag_name_fragment}-appgateway-feip"
    frontend_port_name             = "${var.ag_name_fragment}-appgateway-feport"
    protocol                       = "Http"
  }

  request_routing_rule {
    name                        = "${var.ag_name_fragment}-appgateway-rqrt"
    rule_type                   = "Basic"
    http_listener_name          = "${var.ag_name_fragment}-appgateway-httplstn"
    redirect_configuration_name = "${var.ag_name_fragment}-appgateway-http-redirect"
    rewrite_rule_set_name       = "${var.ag_name_fragment}-appgateway-rewrite-rules"
    priority                    = 10010
  }

  http_listener {
    name                           = "${var.ag_name_fragment}-appgateway-httpslstn"
    frontend_ip_configuration_name = "${var.ag_name_fragment}-appgateway-feip"
    frontend_port_name             = "${var.ag_name_fragment}-appgateway-feporthttps"
    protocol                       = "Https"
    ssl_certificate_name           = var.ssl_cert_name
  }

  request_routing_rule {
    name                        = "${var.ag_name_fragment}-appgateway-rqrt-https"
    rule_type                   = "Basic"
    http_listener_name          = "${var.ag_name_fragment}-appgateway-httpslstn"
    backend_address_pool_name   = "${var.ag_name_fragment}-appgateway-beap"
    backend_http_settings_name  = "${var.ag_name_fragment}-appgateway-be-htst"
    rewrite_rule_set_name       = "${var.ag_name_fragment}-appgateway-rewrite-rules"
    priority                    = 10020
  }

  redirect_configuration {
    name = "${var.ag_name_fragment}-appgateway-http-redirect"
    redirect_type = "Permanent"
    target_listener_name = "${var.ag_name_fragment}-appgateway-httpslstn"
    include_path = true
    include_query_string = true
  }

  # Rewrite rules
  rewrite_rule_set {
    name = "${var.ag_name_fragment}-appgateway-rewrite-rules"
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
    identity_ids = [data.azurerm_user_assigned_identity.managed_identity_aad.id] # [azurerm_user_assigned_identity.managed_id.id]
  }

  probe {
    name                                      = "${var.ag_name_fragment}-appgateway-healthprobe"
    host                                      = var.app_service_hostname
    pick_host_name_from_backend_http_settings = false
    path                                      = "/"
    interval                                  = 30
    timeout                                   = 30
    unhealthy_threshold                       = 3
    protocol                                  = "Http"
  }

  # Waf config

  # waf_configuration {
  #   enabled                  = true
  #   file_upload_limit_mb     = 100
  #   firewall_mode            = "Prevention"
  #   max_request_body_size_kb = 128
  #   request_body_check       = true
  #   rule_set_type            = "OWASP"
  #   rule_set_version         = "3.1"

  #   disabled_rule_group {
  #     rule_group_name = "REQUEST-942-APPLICATION-ATTACK-SQLI"
  #     rules           = [
  #       942430,
  #       942130,
  #       942450,
  #       942440,
  #       942210,
  #       942380,
  #       942200,
  #       942220,
  #       942400
  #     ]
  #   }
  #   disabled_rule_group {
  #     rule_group_name = "REQUEST-920-PROTOCOL-ENFORCEMENT"
  #     rules           = [ 920230 ]
  #   }
  #   disabled_rule_group {
  #     rule_group_name = "REQUEST-931-APPLICATION-ATTACK-RFI"
  #     rules           = [ 931130 ]
  #   }
  #   disabled_rule_group {
  #     rule_group_name = "REQUEST-932-APPLICATION-ATTACK-RCE"
  #     rules           = [ 932115 ]
  #   }
  # }

  tags = {
    environment  = var.environment,
    architecture = "new"
  }

  lifecycle {
    # AGIC owns most app gateway settings, so we should ignore differences
    ignore_changes = [
      #  identity[0].identity_ids,
      #  request_routing_rule, 
      #  http_listener, 
      #  backend_http_settings, 
      #  frontend_port,
      #  backend_address_pool,
      #  probe,
      #  redirect_configuration,      
      #  url_path_map,     
      #  custom_error_configuration,
      #  tags, 
    ]
  }
}
