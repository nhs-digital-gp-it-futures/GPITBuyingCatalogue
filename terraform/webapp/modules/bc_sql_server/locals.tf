locals {
  diagnostics_name   = "${var.project}-${var.environment}-sqldiag"
  target_resource_id = "${azurerm_mssql_server.sql_server.id}/databases/master"
  enable_logs        = [
    "Deadlocks",
    "Errors",
    "SQLSecurityAuditEvents",
    "Timeouts"
  ]
}
