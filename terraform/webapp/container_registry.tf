resource "azurerm_container_registry_task" "registry_purge" {
  provider              = azurerm.acr
  name                  = "purge-old-images"
  container_registry_id = data.azurerm_container_registry.acr.id

  # Overall ACR task timeout: one hour
  timeout_in_seconds = 3600

  platform {
    os           = "Linux"
    architecture = "amd64"
  }

  encoded_step {
    task_content = base64encode(<<-YAML
      version: v1.1.0
      stepTimeout: 3600
      steps:
        - cmd: >-
            mcr.microsoft.com/acr/acr-cli:0.19 purge
            --registry $RegistryName
            --filter '.*:.*'
            --ago 14d
            --keep 10
            --untagged
    YAML
    )
  }

  timer_trigger {
    name     = "daily"
    schedule = "0 2 * * *"
  }
}
