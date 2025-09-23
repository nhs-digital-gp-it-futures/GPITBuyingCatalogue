locals {
  environment_short_name = substr(var.environment, 0, 3)
  project_environment    = "${var.project}-${var.environment}"
  project_short_code     = substr(var.project, 0, 5)
  project_alt_code       = substr(var.project, 0, 4)
  send_notification      = "send-email-notification"
  complete_notification  = "complete-email-notification"
  role_definition_prefix = "Storage Service Properties Reader"
  role_definition_name   = var.environment == "production" || var.environment == "preproduction" ? "${local.role_definition_prefix} (Prod)" : "${local.role_definition_prefix} (Test)"
}
