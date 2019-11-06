using System.IO;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace NexmoNccoVbc.Controllers
{
    public class WebhookController : Controller
    {
        private static object obj_locker = new object();
        private static object obj_locker1 = new object();

        [HttpPost]
        public string NexmoEvents()
        {
            var message = string.Empty;
            using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
            {
                message = reader.ReadToEnd();
            }

            lock (obj_locker)
            {
                //System.IO.File.AppendAllText(@"C:\tmp\nexmoevent.txt", "Nexmo Events:" + message);
            }
            return "200";
        }

        [HttpGet]
        public string NexmoAnswer()
        {
            var from = Request.QueryString["from"];
            var to = Request.QueryString["to"];
            var legId = Request.QueryString["uuid"];
            var callId = Request.QueryString["conversation_uuid"];

            var response = GetAnswerNCCO(from);

            Response.ContentType = "application/json";
            return response;
        }

        [HttpPost]
        public string NexmoFallback()
        {
            var message = string.Empty;
            using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
            {
                message = reader.ReadToEnd();
            }

            lock (obj_locker1)
            {
               // System.IO.File.AppendAllText(@"C:\tmp\nexmoevent.txt", "Nexmo Fallback:" + message);
            }

            var eventJson = JObject.Parse(message);

            var from = (string)eventJson["from"];
            var status = (string)eventJson["status"];

            if (status != "timeout" &&
                status != "failed" &&
                // status != "rejected" ||
                status != "unanswered" &&
                status != "busy")
            {
                return "200";
            }

            var response = GetFallbackNCCO(from);

            Response.ContentType = "application/json";
            return response; 
        }

        private string GetFallbackNCCO(string from)
        {
            var connectCallGroup = new JObject()
            {
                new JProperty("action", "connect"),
                new JProperty("from", $"{from}GG"),
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
                connectCallGroup,
            }.ToString();
        }

        private string GetAnswerNCCO(string clientPhone)
        {
            var welcome = new JObject()
            {
                new JProperty("action", "talk"),
                new JProperty("text", "Thank you for calling to Test Nexmo With Ucallz"),
            };

            var connectCallQueue = new JObject()
            {
                new JProperty("action", "connect"),
                new JProperty("from", $"{clientPhone}QQ"),
                new JProperty("timeout", "10"),
                new JProperty("eventType", "synchronous"),
                new JProperty("eventUrl", new JArray()
                {
                    new JValue("http://4537de78.ngrok.io/Webhook/NexmoFallback"),
                }),
                new JProperty("endpoint", new JArray
                {
                    new JObject()
                    {
                        new JProperty("type", "vbc"),
                        new JProperty("extension", "415")
                    }
                })
            };

            return new JArray
            {
                welcome,
                connectCallQueue,
            }.ToString();
        }
    }
}