using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project8.Models
{
    public class RequestInfo
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public DateTime ReqDateTime { get; set; }
        public string UserName { get; set; }
        public bool Approved { get; set; }
    }
}