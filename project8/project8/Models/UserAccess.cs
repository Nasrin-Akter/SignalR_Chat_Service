using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project8.Models
{
    public class UserAccess
    {
        public int ID { get; set; }
        public string RoleName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string MenuItem { get; set; }
    }
}