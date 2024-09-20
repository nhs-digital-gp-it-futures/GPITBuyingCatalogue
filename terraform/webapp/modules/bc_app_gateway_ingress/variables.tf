variable "project" {
  type = string
}
variable "environment" {
  type = string
}
variable "region" {
  type = string
}
variable "rg_name" {
  type = string
}
variable "ag_capacity" {
  type = string
}
variable "ag_subnet_id" {
  type = string
}
variable "ssl_cert_name" {
  type = string
}
variable "ssl_cert_secret_id" {
  type = string
}
variable "dns_name" {
  type = string
}
variable "managed_id_principal_id" {
  type = string
}
variable "app_service_hostname" {
  type = string
}

variable "app_dns_url" {
  type = string
}

variable "core_env" {
  type = string
}

variable "log_analytics_workspace_id" {
  type = string
}
