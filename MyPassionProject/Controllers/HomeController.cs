using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPassionProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //send request event data controller -> list event
            // receieve a list of events
            // make two variable
            // one for fun type event , one for fare type event
            // Then pass these two variable into a view model
            // return this view model to view

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}