using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitterDLRESBot.Models;
using TwitterDLRESBot.Services;

namespace TwitterDLRESBot.Controllers
{
    public class ValuesController : ApiController
    {
        public static string directLineSecret = ConfigurationManager.AppSettings["directLineSecret"];
        public static string directLineWrapperUrl = ConfigurationManager.AppSettings["directLineWrapperUrl"];
        
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public Response Get(string id)
        {
            Response responseobj = new Response();
            responseobj.speech = "Test response from API";
            responseobj.displayText = "Test response from API";
            responseobj.data = null;
            responseobj.contextOut = null;
            responseobj.source = "directline";
            responseobj.followupEvent = null;
            return responseobj;
        }

        // POST api/values
        public Response Post()
        {
            Response responseobj = new Response();
            DirectlineRequest requestobj = new DirectlineRequest();
            string userID = string.Empty;
            string userMessage = string.Empty;
            try
            {
                string content = Request.Content.ReadAsStringAsync().Result;
                //var requestData = (JObject)JsonConvert.DeserializeObject(content);
                JObject requestData = JObject.Parse(content);
                JToken objJToken;
                if(requestData.TryGetValue("originalRequest", out objJToken))
                userID =(string)requestData["originalRequest"]["data"]["user"]["id_str"];

                userMessage = (string)requestData["result"]["resolvedQuery"];
                Trace.TraceInformation("API.AI Params : " + content);

                //call directline API wrapper
                requestobj.userID = userID;
                requestobj.userMessage = userMessage;
                requestobj.startConversation = true;
                requestobj.directLineSecret = directLineSecret;
                responseobj.speech =DirectlineAPIWrapper.GetResponseFromDirectlineAPI(requestobj, directLineWrapperUrl);
                responseobj.displayText = "Test response from API for display text";
                responseobj.data = null;
                responseobj.contextOut = null;
                responseobj.source = "twitter";
                responseobj.followupEvent = null;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Exception : " + ex.Message);
            }
            return responseobj;

        }



        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
