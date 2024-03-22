using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetBoardingApp.Controllers
{
    public class UserController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View("~/Views/Users/UserHome.cshtml");
        }

        [Authorize]
        public ActionResult Pets()
        {
            return View("~/Views/Pets/Index.cshtml");
        }
    }
}