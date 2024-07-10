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
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using static System.Net.Mime.MediaTypeNames;
using System.Net;


namespace MyPassionProject.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static HomeController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri(Constant.BaseUrl);

        }

        [HttpPost]
        public ActionResult SendEmail(string toAddress, string subject, string body)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"To: {toAddress}");
                System.Diagnostics.Debug.WriteLine($"Subject: {subject}");
                System.Diagnostics.Debug.WriteLine($"Body: {body}");

                string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
                int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
                string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Community Booster", smtpUsername)); // Display name and sender email
                message.To.Add(new MailboxAddress("", toAddress)); // Recipient email

                message.Subject = subject;
                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(smtpServer, smtpPort, useSsl: true);
                    client.Authenticate(smtpUsername, smtpPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return Content("Email sent successfully!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                Response.StatusCode = 400; // Bad Request status code
                return Content($"Error sending email: {ex.Message}");
            }
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
        // GET: Home/Account
        [HttpGet]
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult MyEvents()
        {
            //use HTTP client to access infomation
            //objective: communicate with our event data api to retrieve a list of event
            //curl https://localhost:44317/api/EventData/ListEventsForApplicationUser
            //string CurrentUserId = User.Identity.GetUserId();

            //string url = "EventData/ListEventsForApplicationUser?CurrentUserId={CurrentUserId}";
            //HttpResponseMessage response = client.GetAsync(url).Result;//According to your method, use GetAsync,PostAsync,or ReadAsAsync.
            //List<EventDto> Events = response.Content.ReadAsAsync<List<EventDto>>().Result;


            //return View(Events);

            string CurrentUserId = User.Identity.GetUserId();
            Debug.WriteLine("what is " + CurrentUserId);
            string url = $"EventData/ListEventsForApplicationUser?CurrentUserId={CurrentUserId}";
            Debug.WriteLine("It is going to" + url);
            HttpResponseMessage response = null;
            List<EventDto> Events = null;
            response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode(); // Ensure HTTP 200 OK status
            Events = response.Content.ReadAsAsync<List<EventDto>>().Result;

            Debug.WriteLine("Again, what is " + CurrentUserId);
            string listcreatedUrl = $"EventData/ListCreatedEventForUser?CurrentUserId={CurrentUserId}";
            Debug.WriteLine("It is going to" + listcreatedUrl);
            HttpResponseMessage listcreatedUrlResponse = client.GetAsync(listcreatedUrl).Result;
            List<EventDto> CreatedEvents = listcreatedUrlResponse.Content.ReadAsAsync<List<EventDto>>().Result;



            var findEvent = new FindEvent
            {
                CurrentUserId = CurrentUserId,
                RelatedEvent = Events,
                CreatedEvents = CreatedEvents
            };
            return View(findEvent);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Associate(int EventId, string CurrentUserId)
        {

            string convertedEventId = EventId.ToString();


            string associateUrl = "EventData/AssociateEventWithApplicationUser/" + convertedEventId + "/" + CurrentUserId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage associateResponse = client.PostAsync(associateUrl, content).Result;

            return RedirectToAction("MyEvents");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UnAssociate(int EventId, string CurrentUserId)
        {
            string convertedEventId = EventId.ToString();

            string unassociateUrl = "EventData/UnAssociateEventWithApplicationUser/" + convertedEventId + "/" + CurrentUserId;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage unassociateResponse = client.PostAsync(unassociateUrl, content).Result;

            return RedirectToAction("MyEvents");
        }
        [HttpGet]
        public ActionResult Search(string query)
        {
            Debug.WriteLine("Attempt to Search");
            string url = $"EventData/SearchEvent?query={query}";

            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode(); // Ensure that the HTTP request was successful
                List<EventDto> RelatedEvent = response.Content.ReadAsAsync<List<EventDto>>().Result;

                // Pass the CreatedEvents list to the view for rendering
                return View("Search", RelatedEvent);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception or handle it appropriately
                Debug.WriteLine($"HTTP request failed: {ex.Message}");
                return View("Error");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult Sort(string visibleCategory, string sortType)
        {
            Debug.WriteLine("Attempt to sort");

            // Check if either visibleCategory or sortType is null or empty
            if (string.IsNullOrEmpty(visibleCategory) || string.IsNullOrEmpty(sortType))
            {
                return RedirectToAction("Error");
            }

            try
            {
                // Assuming 'client' is an HttpClient or similar setup to make HTTP requests
                string url1 = "EventData/ListEventsForCategory/1";
                HttpResponseMessage response1 = client.GetAsync(url1).Result;
                List<EventDto> FunEvents = response1.Content.ReadAsAsync<List<EventDto>>().Result;

                string url2 = "EventData/ListEventsForCategory/2";
                HttpResponseMessage response2 = client.GetAsync(url2).Result;
                List<EventDto> FareEvents = response2.Content.ReadAsAsync<List<EventDto>>().Result;

                // Determine which list of events to sort based on visibleCategory
                List<EventDto> eventsToSort = visibleCategory == "Fun" ? FunEvents : FareEvents;

                // Normalize sortType to lowercase for consistency
                sortType = sortType.ToLower();

                // Switch statement to handle different sorting types
                switch (sortType)
                {
                    case "distance":
                        ViewBag.sortType = "distance"; // Set ViewBag.SortType for use in view
                        break;
                    case "date":
                        ViewBag.sortType = "date"; // Set ViewBag.SortType for use in view
                        break;
                    default:
                        ViewBag.sortType = "default"; // Default case, could be used for other scenarios
                        break;
                }

                // Create view model with necessary data to pass to the partial view
                var viewModel = new EventsForCategory
                {
                    FunEvents = FunEvents,
                    FareEvents = FareEvents,
                    SortedEvents = eventsToSort,
                    visibleCategory = visibleCategory,
                    SortType = sortType // Assign SortType to the view model
                };

                // Return partial view with sorted events and view model
                return PartialView("_Sort", viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error");
            }
        }

    }
}