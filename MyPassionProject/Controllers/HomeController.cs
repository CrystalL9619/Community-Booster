using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using MyPassionProject.Models;
using MyPassionProject.Models.ViewModels;

namespace MyPassionProject.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static HomeController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("https://localhost:44317/api/");
        }
        public ActionResult Index()
        {
            //send request event data controller -> list event for category
            //use HTTP client to access infomation
            //objective: communicate with our event data api to retrieve a list of event
            // make two variable
            // one for EventData/ListEventsForCategory/1 event , one for EventData/ListEventsForCategory/2 event

            string url1 = "EventData/ListEventsForCategory/1";
            HttpResponseMessage response1 = client.GetAsync(url1).Result;
            List<EventDto> FunEvents = response1.Content.ReadAsAsync<List<EventDto>>().Result;

            string url2 = "EventData/ListEventsForCategory/2";
            HttpResponseMessage response2 = client.GetAsync(url2).Result;
            List<EventDto> FareEvents = response2.Content.ReadAsAsync<List<EventDto>>().Result;

            // Prepare view model
            var Index = new EventsForCategory
            {
                FunEvents = FunEvents,
                FareEvents = FareEvents
            };



            // return this view model to view

            return View(Index);
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