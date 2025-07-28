variable "environment" {
  type = string
}

variable "project" {
  type = string
}

variable "sql_collation" {
  type = string
}

variable "rg_replica_name" {
  type = string
}

variable "sqlsvr_replica_name" {
  type = string
}

variable "enable_replica" {
  type = number
}

variable "server_id" {
  type = string
}

variable "is_live_environment" {
  type = bool
}

variable "log_analytics_workspace_id" {
  type = string
}