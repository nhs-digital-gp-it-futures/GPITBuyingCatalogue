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
variable "sqlsvr_name" {
  type = string
}
variable "sql_collation" {
  type = string
}
variable "sql_edition" {
  type = string
}
variable "sql_size" {
  type = string
}
variable "region_replica" {
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

variable "core_env" {
  type = string
}
