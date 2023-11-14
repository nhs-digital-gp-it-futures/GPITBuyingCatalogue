variable "environment" {
  type = string
}

variable "project" {
  type = string
}

variable "region" {
  type = string
}

variable "rg_name" {
  type = string
}

variable "webapp_name" {
  type = string
}

variable "sku_tier" {
  type = string
}

variable "sku_size" {
  type = string
}

variable "repository_name" {
  type = string
}

variable "always_on" {
  type = string
}

variable "cert_name" {
  type = string
}

variable "aspnet_environment" {
  type = string
}

variable "instrumentation_key" {
  type = string
}

variable "primary_vpn" {
  type = string
}

variable "app_gateway_ip" {
  type = string
}

variable "app_dns_url" {
  type = string
}

variable "docker_registry_server_url" {
  type = string
}

variable "docker_registry_server_username" {
  type = string
}

variable "docker_registry_server_password" {
  type = string
}

variable "create_slot" {
  type = string
}

variable "create_host_binding" {
  type = string
}

variable "ssl_thumbprint" {
  type = string
}

variable "notify_api_key" {
  type = string
}

# SQL Variables
variable "sqlserver_name" {
  type = string
}

variable "sqlserver_rg" {
  type = string
}

variable "db_name_main" {
  type = string
}

variable "sql_admin_username" {
  type = string
}

variable "sql_admin_password" {
  type      = string
  sensitive = true
}

variable "blob_storage_connection_string" {
  type      = string
  sensitive = true
}

variable "recaptcha_site_key" {
  type = string
}

variable "recaptcha_secret_key" {
  type = string
}