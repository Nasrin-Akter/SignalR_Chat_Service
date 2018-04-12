using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project8.Models
{
    public class MessageInfo
    {
        public int ID { get; set; }
        public string MessageBody { get; set; }
        public string PostDateTime { get; set; }
        public string UserName { get; set; }
    }
}