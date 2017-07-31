using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        public static SqlConnection GetSqlConnection()
        {
            //Dev
            string ADOConnectionstring = "Server=tcp:dlbotdbs1.database.windows.net,1433;Initial Catalog=DLBotDB;Persist Security Info=False;User ID=DlBotdbs1;Password=delta@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //PROD
            //string ADOConnectionstring = "Server=tcp:dlbotdbs1.database.windows.net,1433;Initial Catalog=Lpsmldb;Persist Security Info=False;User ID=DlBotdbs1;Password=delta@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var connection = new SqlConnection(ADOConnectionstring);
            connection.Open();
            return connection;
        }
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
            string reply = string.Empty;
           
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
                //Reset response table
                //DeleteResponseContextForBot(userID);
                //call directline API wrapper
                requestobj.userID = userID;
                requestobj.userMessage = userMessage;
                requestobj.startConversation = true;
                requestobj.directLineSecret = directLineSecret;
                //reply= DirectlineAPIWrapper.GetResponseFromDirectlineAPI(requestobj, directLineWrapperUrl);
                reply = APIIntegration.GetReplyFromBot(userMessage, directLineSecret, true, userID);
                responseobj.speech = reply;
                responseobj.displayText = "DisplayText";
                responseobj.data = null;
                responseobj.contextOut = null;
                responseobj.source = "twitter";
                responseobj.followupEvent = null;
                Trace.TraceInformation("Reply from bot : " + reply);
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

        public static void DeleteResponseContextForBot(string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("DeleteResponseContextForBot", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                        connection.Close();
                }

            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
    }
}
