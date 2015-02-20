namespace Presentation.Web.Test.Controllers.Models
{
    using System.Collections.Generic;

    public class ODataMetaData
    {
        public string odatacontext { get; set; }
        public List<Value> value { get; set; }
    }

    public class Value
    {
        public string name { get; set; }
        public string kind { get; set; }
        public string url { get; set; }
    }
}
