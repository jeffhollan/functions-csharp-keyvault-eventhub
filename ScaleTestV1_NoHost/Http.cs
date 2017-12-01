using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.EventHubs;
using System.Text;
using System;

namespace ScaleTestV1_NoHost
{
    public static class Http
    {
        private static HttpClient client = new HttpClient();
        private static EventHubClient eventHubClient;

        public static string ConfigurationManager { get; private set; }

        [FunctionName("Http")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            if(eventHubClient == null)
            {
                log.Info("Retrieving secret from keyvault");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback), client);
                log.Info("Got secret");
                string eventHubConnectionString = (await kvClient.GetSecretAsync(Environment.GetEnvironmentVariable("EventHubSecretId"))).Value;
                eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString);
                log.Info("Created event hub client");
            }

            JObject message = JObject.FromObject(new
            {
                data = await req.Content.ReadAsAsync<JObject>()
            });
            log.Info("Sending to event hub");
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message.ToString())));

            log.Info($"Sent message to Event Hub");
            return req.CreateResponse(HttpStatusCode.OK, message);
        }
    }
}
