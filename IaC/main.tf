terraform {

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.0.0"

    }
  }

}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "perftestgroup" {
  name     = "rg-${var.projectnamingconvention}-1"
  location = var.location
}

resource "azurerm_storage_account" "storage" {
  name                     = "sto${var.projectnamingconvention}1"
  resource_group_name      = azurerm_resource_group.perftestgroup.name
  location                 = azurerm_resource_group.perftestgroup.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "functionserviceplan" {
  name                = "asp-${var.projectnamingconvention}-1"
  resource_group_name = azurerm_resource_group.perftestgroup.name
  location            = azurerm_resource_group.perftestgroup.location
  
  maximum_elastic_worker_count = 100
  
  os_type             = "Linux"
  sku_name            = "EP3"
}

resource "azurerm_linux_function_app" "perftestfunction" {
  name                = "func-${var.projectnamingconvention}-Central-1"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  service_plan_id     = azurerm_service_plan.functionserviceplan.id

  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key

  site_config {
    application_stack {
      python_version = "3.9"
    }
    elastic_instance_minimum = 20
  }
}

resource "azurerm_function_app_function" "basicapp" {
  name            = "app-${var.projectnamingconvention}-1"
  function_app_id = azurerm_linux_function_app.perftestfunction.id
  language        = "Python"
  test_data = jsonencode({
    "name" = "Azure"
  })
  config_json = jsonencode({
    "bindings" = [
      {
        "authLevel" = "function"
        "direction" = "in"
        "methods" = [
          "get"
        ]        
        "name" = "req"
        "type" = "httpTrigger"
      },
      {
        "direction" = "out"
        "name"      = "$return"
        "type"      = "http"
      },
    ]
  })
}
