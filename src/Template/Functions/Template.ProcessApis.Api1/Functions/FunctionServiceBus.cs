using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Template.Domain.Constants;
using Template.ProcessApis.Api1.RequestModel;
using Template.ProcessApis.Api1.Services;

namespace Template.ProcessApis.Api1.Functions
{
    internal class FunctionServiceBus
    {
        private readonly IService1 _service1;

        public FunctionServiceBus(IService1 service1)
        {
            _service1 = service1 ?? throw new ArgumentException(nameof(service1));
        }

        [FunctionName("FunctionServiceBus")]
        public void Run([ServiceBusTrigger("facturation", Connection = "ServiceBusConnection")]string queueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {queueItem}");

            if(string.IsNullOrWhiteSpace(queueItem))
            {
                log.LogError(Errors.BUS_EMPTY_ITEM);
                throw new ArgumentException(nameof(queueItem));
            }

            OrderRequest order;
            if ((order = JsonConvert.DeserializeObject<OrderRequest>(queueItem)) == null)
            {
                throw new ApplicationException(Errors.BUS_DESERIALIZATION);
            }

            try
            {
                _service1.Save(order);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Erreur lors de l'enregistrement");
            }
            
        }
    }
}
