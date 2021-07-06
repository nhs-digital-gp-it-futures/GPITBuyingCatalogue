resource "azurerm_application_gateway" "AppGw" {
  name                             = var.ag_name
  location                         = var.region
  resource_group_name              = var.rg_name 

  sku {
    name                           = "Standard_v2"
    tier                           = "Standard_v2"
    capacity                       = var.ag_capacity
  }

  custom_error_configuration {
    status_code                   = "HttpStatus502"
    custom_error_page_url         = "https://gpitfuturesappgwcontent.z33.web.core.windows.net/index.html"
  }

  gateway_ip_configuration {
    name                           = "${var.ag_name_fragment}-gwip"
    subnet_id                      = var.ag_subnet_id
  }

  frontend_ip_configuration {
    name                           = "${var.ag_name_fragment}-appgw-feip"
    public_ip_address_id           = azurerm_public_ip.PipAppGw.id
  }

  backend_address_pool {
    name                           = "${var.ag_name_fragment}-appgw-beap"
  }

  backend_http_settings {
    name                           = "${var.ag_name_fragment}-appgw-be-htst"
    cookie_based_affinity          = "Disabled"
    path                           = "/path/"
    port                           = 80
    protocol                       = "Http"
    request_timeout                = 1
  }

  frontend_port {
    name                           = "${var.ag_name_fragment}-appgw-feport"
    port                           = 80
  }

  frontend_port {
    name                           = "${var.ag_name_fragment}-appgw-feporthttps"
    port                           = 443
  }

  ssl_policy {
    policy_type                   = "Predefined"
    policy_name                   = "AppGwSslPolicy20170401S"
  }

  # Static Sites

  http_listener {
    name                           = "${var.ag_name_fragment}-appgw-httplstn"
    frontend_ip_configuration_name = "${var.ag_name_fragment}-appgw-feip"
    frontend_port_name             = "${var.ag_name_fragment}-appgw-feport"
    protocol                       = "Http"
  }

  request_routing_rule {
    name                          = "${var.ag_name_fragment}-appgw-rqrt"
    rule_type                     = "Basic"
    http_listener_name            = "${var.ag_name_fragment}-appgw-httplstn"
    backend_address_pool_name     = "${var.ag_name_fragment}-appgw-beap"
    backend_http_settings_name    = "${var.ag_name_fragment}-appgw-be-htst"
  }

  http_listener {
    name                           = "${var.ag_name_fragment}-appgw-httpslstn"
    frontend_ip_configuration_name = "${var.ag_name_fragment}-appgw-feip"
    frontend_port_name             = "${var.ag_name_fragment}-appgw-feporthttps"
    protocol                       = "HTTPS"
    host_name                      = var.core_url
    ssl_certificate_name           = var.ssl_cert_name
  }

  request_routing_rule {
    name                          = "${var.ag_name_fragment}-appgw-rqrt-https"
    rule_type                     = "Basic"
    http_listener_name            = "${var.ag_name_fragment}-appgw-httpslstn"
    backend_address_pool_name     = "${var.ag_name_fragment}-appgw-beap"
    backend_http_settings_name    = "${var.ag_name_fragment}-appgw-be-htst"
  }

  # Rewrite rules
  rewrite_rule_set {
    name                          = "${var.ag_name_fragment}-appgw-rewrite-rules"
    rewrite_rule {
      name                        = "StrictTransportSecurityRule"
      rule_sequence               = 1
      response_header_configuration {
        header_name               = "Strict-Transport-Security"
        header_value              = "max-age=86400; includeSubDomains"
      }
    }
  }   
  # Redirect
  
  http_listener {
    name                           = "${var.ag_name_fragment}-appgw-pub-httpslstn"
    frontend_ip_configuration_name = "${var.ag_name_fragment}-appgw-feip"
    frontend_port_name             = "${var.ag_name_fragment}-appgw-feporthttps"
    protocol                       = "HTTPS"
    host_name                      = "www.${var.core_url}" 
    ssl_certificate_name           = var.ssl_cert_name
  }

  http_listener {
    name                           = "${var.ag_name_fragment}-appgw-pub-httplstn"
    frontend_ip_configuration_name = "${var.ag_name_fragment}-appgw-feip"
    frontend_port_name             = "${var.ag_name_fragment}-appgw-feport"
    protocol                       = "HTTP"
    host_name                      = "www.${var.core_url}"
  }

  redirect_configuration {
    name                          = "${var.ag_name_fragment}-appgw-rdrcfg"
    redirect_type                 = "Permanent"
    target_url                    = "https://${var.core_url}"
    include_path                  = true
    include_query_string          = true
  }

  request_routing_rule {
    name                          = "${var.ag_name_fragment}-appgw-pub-httpsrqrt"
    rule_type                     = "Basic"
    http_listener_name            = "${var.ag_name_fragment}-appgw-pub-httpslstn"
    redirect_configuration_name   = "${var.ag_name_fragment}-appgw-rdrcfg"
  }

  request_routing_rule {
    name                          = "${var.ag_name_fragment}-appgw-pub-httprqrt"
    rule_type                     = "Basic"
    http_listener_name            = "${var.ag_name_fragment}-appgw-pub-httplstn"
    redirect_configuration_name   = "${var.ag_name_fragment}-appgw-rdrcfg"
  }
  
  ssl_certificate {
    name                         = var.ssl_cert_name
    key_vault_secret_id          = var.ssl_cert_secret_id
  }

  identity {
    type                          = "UserAssigned"
    identity_ids                  = [data.azurerm_user_assigned_identity.managed_identity_aad.id] # [azurerm_user_assigned_identity.managed_id.id]
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
    environment                 = var.environment
  }

  lifecycle {
    # AGIC owns most app gateway settings, so we should ignore differences
    ignore_changes = [
      identity[0].identity_ids,
      request_routing_rule, 
      http_listener, 
      backend_http_settings, 
      frontend_port,
      backend_address_pool,
      probe,
      redirect_configuration,      
      url_path_map,     
      custom_error_configuration,
      tags, 
    ]
  }
}
