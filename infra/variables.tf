################
## General #####
################

variable "resource_group_name" {
  description = "Le nom du groupe de ressources dans lequel seront placées toutes les ressources."
}

#############
## Tags #####
#############

variable "environment" {
  description = "Environnement cible pour toutes les ressources"
  default     = "dev"
}

variable "contact" {
  description = "Service demandeur auquel on peut se référer en cas de besoin : Marketing, Commerce, … "
  default     = "Equipe Proxaffiche"
}

##########################
## App Configuration #####
##########################

variable "app_configuration_name" {
  description = "Nom de la ressource App Configuration."
}

variable "app_configuration_sku" {
  description = "Sku de la ressource App Configuration."
  default     = "free"
}

####################
## Service bus #####
####################

variable "service_bus_name" {
  description = "Nom de la ressource Service Bus."
}

variable "service_bus_sku_name" {
  description = "Sku de la ressource Service Bus."
  default     = "basic"
}