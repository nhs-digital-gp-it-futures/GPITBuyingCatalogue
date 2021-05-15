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
variable "storage_account_name" {
  type = string
}
variable "replication_type" {
  type = string
}
variable "container_name" {
  type = string
}
variable "ip_rules" {
  type = list(string)
}
variable "aks_subnet_id" {
  type = string
}
variable "kv_id" {
  type = string
}
variable "kv_key" {
  type = string
}
