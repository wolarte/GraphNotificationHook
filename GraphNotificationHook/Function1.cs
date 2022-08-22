using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;

namespace GraphNotificationHook
{
    public class GraphNotification
    {
        public List<ChangeNotification> value { get; set; }
    }


    public static class Function1
    {
        [FunctionName("GraphNotificationHook")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            log.LogInformation("La función de activación HTTP de C# procesó una solicitud");

            // parse query parameter
            var validationToken = req.Query["validationToken"];
            log.LogInformation(validationToken);
            if (!string.IsNullOrEmpty(validationToken))
            {
                log.LogInformation("ValidationToken: " + validationToken);
                return new ContentResult { Content = validationToken, ContentType = "text/plain" };
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<GraphNotification>(requestBody);
            if (requestBody != null)
            {
                Console.WriteLine("Sin datos en el requestbody");
            }

            if (!data.value.FirstOrDefault().ClientState.Equals("<YourClientStateValue>", StringComparison.OrdinalIgnoreCase))
            {
                //client state is not valid (doesn't much the one submitted with the subscription)
                return new BadRequestResult();
            }
            //do something with the notification data

            return new OkResult();
        }
        public class GraphNotification
        {
            public List<ChangeNotification> value { get; set; }
        }

    }
}
