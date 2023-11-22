locals {
  gateway_logs = [
    "ApplicationGatewayAccessLog",
    "ApplicationGatewayFirewallLog"
  ]

  diagnostics_name            = "${var.project}-${var.environment}-appgwdiag"
  gateway_name                = "${var.project}-${var.environment}-appgateway"
  name_fragment               = "${var.project}-${var.environment}"

  backend_address_pool_name   = "${local.name_fragment}-appgateway-beap"
  backend_http_settings_name  = "${local.name_fragment}-appgateway-be-htst"

  public_ip_name              = "${local.name_fragment}-publicip"

  frontend_ip_name            = "${local.name_fragment}-appgateway-feip"
  http_frontend_port_name     = "${local.name_fragment}-appgateway-feport"
  https_frontend_port_name    = "${local.name_fragment}-appgateway-feporthttps"

  gateway_ip_name             = "${local.name_fragment}-gwip"
  healthprobe_name            = "${local.name_fragment}-appgateway-healthprobe"

  http_listener_name          = "${local.name_fragment}-appgateway-httplstn"
  https_listener_name         = "${local.name_fragment}-appgateway-httpslstn"

  http_redirect_config_name   = "${local.name_fragment}-appgateway-http-redirect"
  http_request_routing_name   = "${local.name_fragment}-appgateway-rqrt"
  https_request_routing_name  = "${local.name_fragment}-appgateway-rqrt-https"
  redirect_ruleset_name       = "${local.name_fragment}-appgateway-rewrite-rules"
}