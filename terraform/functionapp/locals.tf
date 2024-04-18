locals {
  environment_short_name = substr(var.environment, 0, 3)
  project_environment    = "${var.project}-${var.environment}"
  project_short_code     = substr(var.project, 0, 5)
  project_alt_code       = substr(var.project, 0, 4)
  send_notification      = "send-email-notification"
  complete_notification  = "complete-email-notification"
}
