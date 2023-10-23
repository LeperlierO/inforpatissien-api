using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace inforpatissien_api.Models
{
    public class IFPLoginData
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class IFPUserData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public bool owner { get; set; }
        public bool gamer { get; set; }
        public string avatarUrl { get; set; }
        public int difficulty { get; set; }
        public string videoUrl { get; set; }
    }

    public class IFPTokenData
    {
        public string token { get; set; }
        public string userName { get; set; }
        public bool gamer { get; set; }
        public string avatarUrl {get; set;}
        public int difficulty { get; set; }
        public string videoUrl { get; set; }
    }
}