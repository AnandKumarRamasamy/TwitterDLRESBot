using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using TwitterDLRESBot.Models;

namespace TwitterDLRESBot.Services
{
    public class DirectlineAPIWrapper
    {
        public static string GetResponseFromDirectlineAPI(DirectlineRequest requestObj,string url)
        {
            string result = string.Empty;
            string jsonOrder = JsonConvert.SerializeObject(requestObj);
            var DATA = Encoding.UTF8.GetBytes(jsonOrder);

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new System.Uri(url);
            //byte[] cred = UTF8Encoding.UTF8.GetBytes("username:password");
            
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(jsonOrder, UTF8Encoding.UTF8, "application/json");
            HttpResponseMessage messge = client.PostAsync(url, content).Result;

            string description = string.Empty;
            if (messge.IsSuccessStatusCode)
            {
                result = messge.Content.ReadAsStringAsync().Result;
                //var jsonData = JsonConvert.DeserializeObject<ActivityConversation>(result);
                //description = jsonData.id;

            }
            return result;
        }
    }
}