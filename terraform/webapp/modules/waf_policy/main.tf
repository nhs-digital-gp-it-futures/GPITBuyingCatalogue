locals {
  disabled_groups = [
    {
      rule_group_name = "REQUEST-942-APPLICATION-ATTACK-SQLI"
      rules = [
        942380,
        942430,
        942400,
        942440,
        942450,
        942130
      ]
    },
    {
      rule_group_name = "REQUEST-920-PROTOCOL-ENFORCEMENT"
      rules = [
        920230
      ]
    },
    {
      rule_group_name = "REQUEST-931-APPLICATION-ATTACK-RFI"
      rules = [
        931130
      ]
    }
  ]
}

resource "azurerm_web_application_firewall_policy" "waf_policy" {
  name                = "${var.project_name}-${var.environment}-waf-policy"
  resource_group_name = var.resource_group_name
  location            = var.location

  policy_settings {
    enabled                     = true
    mode                        = "Prevention"
    request_body_check          = true
    max_request_body_size_in_kb = 128
  }

  managed_rules {
    exclusion {
      match_variable          = "RequestArgNames"
      selector_match_operator = "Equals"
      selector                = "__RequestVerificationToken"
    }

    exclusion {
      match_variable          = "RequestCookieNames"
      selector_match_operator = "Equals"
      selector                = "buyingcatalogue-cookie-consent"
    }

    managed_rule_set {
      type    = "OWASP"
      version = "3.2"

      dynamic "rule_group_override" {
        for_each = local.disabled_groups
        content {
          rule_group_name = rule_group_override.value.rule_group_name

          dynamic "rule" {
            for_each = rule_group_override.value.rules

            content {
              id      = rule.value
              enabled = false
            }
          }
        }
      }
    }
  }
}
