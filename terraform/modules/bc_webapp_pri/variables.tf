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
variable "db_name" {
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
