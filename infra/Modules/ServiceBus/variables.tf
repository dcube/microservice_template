variable "resource_group_name" {
  description = "Le nom du groupe de ressources dans lequel seront placées toutes les ressources."
}

variable "location" {
  description = "Location dans lequel seront placées toutes les ressources."
}

variable "name" {
  description = "Nom de la resource Service Bus"
}

variable "sku_name" {
  description = "Sku de la resource Service Bus : basic, standard, premium"
}

variable "tags" {
  description = "Tags du service bus"
}

variable "queue_list" {
  type = set(string)
  description = "List des files d'attente à créer."
  default = []
}