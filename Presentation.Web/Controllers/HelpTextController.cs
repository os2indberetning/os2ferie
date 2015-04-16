using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OS2Indberetning.Controllers
{
    public class HelpTextController : ApiController
    {
        // GET api/<controller>/5
        public string Get(string id)
        {
            return ConfigurationManager.AppSettings[id];
        }
    }
}