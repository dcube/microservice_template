#https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/servicebus_namespace
resource "azurerm_servicebus_namespace" "service_bus" {
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = var.sku_name

  tags = var.tags
}

resource "azurerm_servicebus_queue" "queue" {
  for_each = var.queue_list

  name                = each.key
  namespace_id        = azurerm_servicebus_namespace.service_bus.id
}
