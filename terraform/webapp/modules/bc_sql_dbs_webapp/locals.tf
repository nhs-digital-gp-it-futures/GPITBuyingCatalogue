locals {
  diagnostics_name   = "${var.project}-${var.environment}-sqldiag"
  enable_logs        = [
    "Deadlocks",
    "Errors",
    "SQLSecurityAuditEvents",
    "Timeouts"
  ]
}
