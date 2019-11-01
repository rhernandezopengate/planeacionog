using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Filters;

namespace WebAppOpenGate.Controllers
{
    [Authorize]
    public class administracionController : Controller
    {
        // GET: administracion
        [AuthorizeUser(IdOperacion: 1)]
        public ActionResult Index()
        {
            return View();
        }
    }
}