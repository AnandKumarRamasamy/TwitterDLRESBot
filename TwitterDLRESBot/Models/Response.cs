using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterDLRESBot.Models
{
    public class Response
    {
        public string speech { get; set; }
        public string displayText { get; set; }
        public data data { get; set; }
        public string source { get; set; }
        public List<context> contextOut { get; set; }
        public followupEvent followupEvent { get; set; }
    }
    public class context
    {
        public string name { get; set; }
        public string lifespan { get; set; }

    }

  


    public class followupEvent
    {
        public string name { get; set; }
        public data data { get; set; }
    }

}