variable "environment" {
  type = string
}
variable "project" {
  type = string
}
variable "pjtcode" {
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
variable "acr_name" {
  type      = string
}
variable "acr_pwd" {
  type      = string
  sensitive = true
}
variable "acr_rg" {
  type      = string
}
variable "repository_name" {
  type      = string
}
variable "always_on" {
  type = string
}
variable "db_name_main" {
  type = string
}

variable "auth_pwd" {
  type      = string
  sensitive = true
}
variable "cert_name" {
  type      = string
}
variable "webapp_cname_url" {
  type      = string
}
variable "core_environment" {
  type      = string
}
variable "sa_connection_string" {
  type = string
  sensitive = true
}

variable "aspnet_environment" {
  type = string
}
variable "sqlserver_name" {
  type = string
}

variable "sqlserver_rg" {
  type = string
}

variable "instrumentation_key" {
  type = string
}

variable "primary_vpn" {
  type = string
}

variable "secondary_vpn" {
  type = string
}

variable "tertiary_vpn" {
  type = string
}

variable "ssl_cert" {
  type = string
}

variable "customer_network_range" {
  type = string
}
