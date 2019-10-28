using System.IO;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace NexmoNccoVbc.Controllers
{
    public class WebhookController : Controller
    {
        [AllowAnonymous]
        public string NexmoEvents()
        {
            var message = string.Empty;
            using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
            {
                message = reader.ReadToEnd();
            }

            return "200";
        }

        public string NexmoAnswer()
        {

            var from = Request.QueryString["from"];
            var to = Request.QueryString["to"];
            var legId = Request.QueryString["uuid"];
            var callId = Request.QueryString["conversation_uuid"];

            var response = GetNCCO(from);

            Response.ContentType = "application/json";
            return response;

        }

        private string GetNCCO(string clientPhone)
        {
            var welcome = new JObject()
            {
                new JProperty("action", "talk"),
                new JProperty("text", "Thank you for calling to Ucallz"),
            };

            var connectCallQueue = new JObject()
            {
                new JProperty("action", "connect"),
                new JProperty("from", $"{clientPhone}Q"),
                new JProperty("timeout", "15"), 
                new JProperty("endpoint", new JArray
                {
                    new JObject()
                    {
                        new JProperty("type", "vbc"),
                        new JProperty("extension", "415")
                    }
                })
            };

            var connectCallGroup = new JObject()
            {
                new JProperty("action", "connect"),
                new JProperty("from", $"{clientPhone}G"),
                new JProperty("timeout", "20"),
                new JProperty("endpoint", new JArray
                {
                    new JObject()
                    {
                        new JProperty("type", "vbc"),
                        new JProperty("extension", "416")
                    }
                })
            };

            return new JArray
            {
                welcome,
                connectCallQueue,
                connectCallGroup,
            }.ToString();
        }
    }
}