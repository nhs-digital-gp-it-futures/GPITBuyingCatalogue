variable "name" {
  type = string
}

variable "target_resource_id" {
  type = string
}

variable "log_analytics_workspace_id" {
  type = string
}

variable "enable_logs" {
  type    = list(string)
}
