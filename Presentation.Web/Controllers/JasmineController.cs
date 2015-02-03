using System;
using System.Web.Mvc;

namespace OS2Indberetning.Controllers
{
    public class JasmineController : Controller
    {
        public ViewResult Run()
        {
            return View("SpecRunner");
        }
    }
}
