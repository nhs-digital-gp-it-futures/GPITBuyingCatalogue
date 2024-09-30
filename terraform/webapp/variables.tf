variable "environment" {
  type = string
}

variable "region" {
  type = string
}

variable "tenant_id" {
  type = string
}

variable "pjtcode" {
  type = string
}

variable "project" {
  type = string
}

variable "kv_sqlusername" {
  type = string
}

variable "coreurl" {
  type = string
}

variable "certname" {
  type = string
}

variable "vnet_address_space" {
  type = string
}

variable "vnet_gateway_address_space" {
  type = string
}

variable "vnet_backend_address_space" {
  type = string
}
variable "primary_vpn" {
  type = string
}

variable "nhsd_network_range" {
  type = string
}

variable "kv_access_group" {
  type = string
}

variable "sql_admin_group" {
  type = string
}

variable "app_url" {
  type = string
}

variable "acr_subscription_id" {
  type = string
}

variable "dns_subscription_id" {
  type = string
}

variable "notify_api_key" {
  type = string
}

variable "primary_env" {
  type = string
  default = ""
}

variable "recaptcha_site_key" {
  type = string
}

variable "recaptcha_secret_key" {
  type = string
}
