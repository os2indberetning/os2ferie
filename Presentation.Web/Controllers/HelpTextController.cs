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
        public IHttpActionResult Get(string id)
        {
            try
            {
                if(id.IndexOf("PROTECTED", StringComparison.Ordinal) > -1)
                {
                    // If the key contains PROTECTED, then return forbidden.
                    return StatusCode(HttpStatusCode.Forbidden);
                }
                // If the key doesnt contain protected, then return the result.
                var res = ConfigurationManager.AppSettings[id];
                return Ok(res);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}