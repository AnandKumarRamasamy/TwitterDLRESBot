using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterDLRESBot.Models
{
    public class Request
    {
        public string id { get; set; }
        public string timestamp { get; set; }
        public string lang { get; set; }
        public string sessionId { get; set; }
        public result result { get; set; }
        public originalRequest originalRequest { get; set; }
    }
    public class result
    {
        public string source { get; set; }
        public string resolvedQuery { get; set; }
        public string action { get; set; }
        public List<context> contextList { get; set; }
        public bool actionIncomplete { get; set; }

    }

    public class originalRequest
    {
        public data data { get; set; }
        
    }
    public class data
    {
        public user user { get; set; }
    }
    public class user
    {
        public string id_str { get; set; }
        public string screen_name { get; set; }

    }
    public class input
    {
        public string intent { get; set; }

    }
    public class conversation
    {
        public string conversation_id { get; set; }
        public int type { get; set; }
        public string conversation_token { get; set; }

    }

}