variable "storage_account_name" {
  type = string
}

variable "environment" {
  type = string
}

variable "region" {
  type = string
}

variable "resource_group" {
  type = string
}

variable "ip_rules" {
  type = list(string)
}

variable "subnet_ids" {
  type = list(string)
}

variable "principal_id" {
  type = string
}