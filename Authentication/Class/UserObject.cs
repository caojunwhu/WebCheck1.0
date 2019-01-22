using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Authentication.Class
{
    public class UserObject
    {
        public string userid{set;get;}
        public string username { set; get; }
        public string password { set; get; }
        public string ipaddress { set; get; }
        public string macaddress { set; get; }
        public string createtime { set; get; }
        public string authorized { set; get; }
        public string company { set; get; }
    }
}
