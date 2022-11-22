locals {
    project_environment = "${var.project}-${var.environment}"
    project_short_code = substr(var.project, 0, 5)
    project_alt_code = substr(var.project, 0, 4)
}