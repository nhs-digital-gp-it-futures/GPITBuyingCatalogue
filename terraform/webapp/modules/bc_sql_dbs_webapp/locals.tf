locals {
  diagnostics_name = "${var.project}-${var.environment}-sqldiag"
  enable_logs = [
    "Deadlocks",
    "Errors",
    "SQLSecurityAuditEvents",
    "Timeouts"
  ]
  enable_metrics = [
    "Basic",
    "InstanceAndAppAdvanced",
    "WorkloadManagement"
  ]
}
