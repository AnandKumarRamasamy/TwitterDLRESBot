using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterDLRESBot.Models
{
    public class DirectlineRequest
    {
        public string userMessage { get; set; }
        public string directLineSecret { get; set; }
        public bool startConversation { get; set; }
        public string userID { get; set; }
    }
}