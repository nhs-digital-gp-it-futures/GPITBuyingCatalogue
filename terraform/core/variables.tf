variable "domain_name" {
  type = string
}

variable "project" {
  type = string
}

variable "environment" {
  type    = string
  default = "dynamic"
}

variable "subscription_id" {
  type      = string
  sensitive = true
}
