using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Filters;
using Microsoft.AspNet.Identity;

namespace WebAppOpenGate.Controllers
{
    [Authorize]
    public class administracionController : Controller
    {
        private string user;
        public administracionController() 
        {
            this.user = User.Identity.GetUserId().ToString();        
        }

        // GET: administracion
        [AuthorizeUser(IdOperacion: 1)]
        public ActionResult Index()
        {
            User.Identity.GetUserId();
            return View();
        }
    }
}