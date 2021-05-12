output "webapp_URL" {
  value = local.gw_webappURL
}

output "webapp_default_URL" {
  value = module.webapp.webapp_default_site_hostname
}
