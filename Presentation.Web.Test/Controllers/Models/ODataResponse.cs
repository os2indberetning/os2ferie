namespace Presentation.Web.Test.Controllers.Models
{
    using System.Collections.Generic;

    public class ODataResponse<T>
        where T : class, new()
    {
        public string odatacontext { get; set; }
        public List<T> value { get; set; }
    }
}