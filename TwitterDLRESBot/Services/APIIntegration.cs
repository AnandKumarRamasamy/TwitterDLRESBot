using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TwitterDLRESBot.Services
{
    public static class APIIntegration
    {
        public static string GetReplyFromBot(string userMessage, string directLineSecret, bool startConversation, string userID)
        {
            model obj = new model();
            FromUser user = new FromUser();
            user.id = userID;
            obj.from = user;
            obj.text = userMessage;
            obj.type = "message";
            string conversationID = string.Empty;
            conversationID = GetResponse(directLineSecret, "content locker", "https://directline.botframework.com/v3/directline/conversations", string.Empty);
            Trace.TraceError("conversationID : " + conversationID);
            string id = GetResponseCopy(directLineSecret, "content locker", "https://directline.botframework.com/v3/directline/conversations/" + conversationID + "/activities", obj);
            Trace.TraceError("ID : " + id);
            string reply = GetActivitiesCopy("https://directline.botframework.com/v3/directline/conversations/" + conversationID + "/activities", directLineSecret, id);
            return reply;
        }
        private static string GetResponse(string token, string userMessage, string url, string data)
        {
            string URL = url;
            string DATA = data;

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new System.Uri(URL);
            //byte[] cred = UTF8Encoding.UTF8.GetBytes("username:password");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token + "");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(DATA, UTF8Encoding.UTF8, "application/json");
            HttpResponseMessage messge = client.PostAsync(URL, content).Result;

            string description = string.Empty;
            if (messge.IsSuccessStatusCode)
            {
                string result = messge.Content.ReadAsStringAsync().Result;
                var jsonData = JsonConvert.DeserializeObject<Conversation>(result);
                description = jsonData.conversationId;

            }

            return description;

        }

        private static string GetResponseCopy(string token, string userMessage, string url, model obj)
        {


            string jsonOrder = JsonConvert.SerializeObject(obj);
            var DATA = Encoding.UTF8.GetBytes(jsonOrder);

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new System.Uri(url);
            //byte[] cred = UTF8Encoding.UTF8.GetBytes("username:password");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token + "");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(jsonOrder, UTF8Encoding.UTF8, "application/json");
            HttpResponseMessage messge = client.PostAsync(url, content).Result;

            string description = string.Empty;
            if (messge.IsSuccessStatusCode)
            {
                string result = messge.Content.ReadAsStringAsync().Result;
                var jsonData = JsonConvert.DeserializeObject<ActivityConversation>(result);
                description = jsonData.id;

            }

            return description;
        }

        private static string GetActivitiesCopy(string url, string token, string id)
        {
            string reply = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";

            //string message = request.Method + url + request.ContentType + unixdate;

            request.Headers["Authorization"] = "Bearer " + token + "";
            //request.Headers["Content-Type"] = "application/json";


            //post data

            //get response
            WebResponse response = request.GetResponse();
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            if (((HttpWebResponse)response).StatusDescription == "OK")
            {
                using (var stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string description = reader.ReadToEnd();
                    ActivitiesList jsonData = JsonConvert.DeserializeObject<ActivitiesList>(description);
                    foreach (Activities obj in jsonData.activities)
                    {
                        if (obj.replyToId == id)
                        {
                            reply = obj.text;
                        }
                    }
                }
            }


            return reply;
        }
        public class model
        {
            public string type { get; set; }
            public FromUser from { get; set; }
            public string text { get; set; }
        }
        public class FromUser
        {
            public string id { get; set; }
        }
        public class Conversation
        {
            public string conversationId { get; set; }
            public string token { get; set; }
        }
        public class ActivityConversation
        {
            public string id { get; set; }

        }
        public class ActivitiesList
        {
            public List<Activities> activities { get; set; }
        }
        public class Activities
        {
            public string type { get; set; }
            public string id { get; set; }
            public string timestamp { get; set; }
            public string channelId { get; set; }
            public string text { get; set; }
            public string replyToId { get; set; }
            public FromUser from { get; set; }
            public ActivityConversation conversation { get; set; }
        }
    }
}