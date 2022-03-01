# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.98"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

data "azurerm_resource_group" "resource_group" {
  name = var.resource_group_name
}

#https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/app_configuration
resource "azurerm_app_configuration" "app_configuration" {
  name                = var.app_configuration_name
  location            = data.azurerm_resource_group.resource_group.location
  resource_group_name = data.azurerm_resource_group.resource_group.name
  sku                 = var.app_configuration_sku
  tags = {
    Contact      = var.contact
    Environement = var.environment
    Role         = "Centralize service configuration"
  }
}

module "function_service_bus" {

  source = "./Modules/ServiceBus"

  resource_group_name = data.azurerm_resource_group.resource_group.name
  location            = data.azurerm_resource_group.resource_group.location
  name                = var.service_bus_name
  sku_name            = var.service_bus_sku_name
  queue_list          = ["facturation"]

  tags = {
    Contact      = var.contact
    Environement = var.environment
    Role         = "Bus used for asynchronous micro services"
  }
}