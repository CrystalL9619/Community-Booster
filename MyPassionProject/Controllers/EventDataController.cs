using MyPassionProject.Models;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Web;
using System.Threading.Tasks;


namespace MyPassionProject.Controllers
{

    public class EventDataController : ApiController
    {
        //Utlizing the database connection 
        private ApplicationDbContext db = new ApplicationDbContext();
        private object modelBuilder;

        //ListEvents
        //GET:"api/EventData/ListEvents
        [HttpGet]
        [Route("api/EventData/ListEvents")]
        public List<EventDto> ListEvents()
        {
            List<Event> Events = db.Events.ToList();
            List<EventDto> EventDtos = new List<EventDto>();
            Events.ForEach(e => EventDtos.Add(new EventDto()
            {
                EventId = e.EventId,
                Title = e.Title,
                CategoryName = e.Category.CategoryName,
                ImagePath = e.ImagePath
            }));
            return EventDtos;
        }
        /// <summary>
        /// Gathers info about all events related to a particular CategoryId
        /// </summary>
        /// <param name="id">CategoryId</param>
        /// <returns>
        ///</returns>
        ///GET:api/EventData/ListEventsForCategory/1

        [HttpGet]
        [ResponseType(typeof(EventDto))]
        public IHttpActionResult ListEventsForCategory(int id)
        {
            Debug.WriteLine("Attempting to List Events for Category");
            //SQL Equivalent:
            //Select * from events where events.categoryid = {id}

            List<Event> Events = db.Events.Where(e => e.CategoryId == id).ToList();
            List<EventDto> EventDtos = new List<EventDto>();

            Events.ForEach(e => EventDtos.Add(new EventDto()
            {
                EventId = e.EventId,
                Title = e.Title,
                Location = e.Location,
                EventDateTime = e.EventDateTime,
                Capacity = e.Capacity,
                Details = e.Details,
                CategoryId = e.Category.CategoryId,  // Use e.CategoryId instead of e.Category.CategoryId
                CategoryName = e.Category.CategoryName
            }));

            return Ok(EventDtos);
        }
        /// <summary>
        /// Gathers info about all events related to a particular UserId
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns>
        ///</returns>
        ///GET:api/EventData/ListEventsForAppUser/1
        [HttpGet]
        [ResponseType(typeof(EventDto))]
        public IHttpActionResult ListEventsForAppUser(int id)
        {
            //SQL equivalent:
            //select events.*,eventAppUsers.* from events INNER JOIN 
            //eventAppUsers on events.eventId = eventAppUsers.eventId
            //where eventAppUsers.UserId={USERID}

            //all events that have users which match with our ID
            List<Event> Events = db.Events.Where(
                e => e.AppUsers.Any(
                    a => a.UserId == id
                )).ToList();
            List<EventDto> EventDtos = new List<EventDto>();

            Events.ForEach(e => EventDtos.Add(new EventDto()
            {
                EventId = e.EventId,
                Title = e.Title,
                Location = e.Location,
                EventDateTime = e.EventDateTime,
                Capacity = e.Capacity,
                Details = e.Details,
                CategoryId = e.Category.CategoryId,
                CategoryName = e.Category.CategoryName
            }));

            return Ok(EventDtos);
        }

        //AssociateEventWithAppUser
        //api/eventData/AssociateEventWithAppUser/9/1
        [HttpPost]
        [Route("api/EventData/AssociateEventWithAppUser/{EventId}/{UserId}")]
        public IHttpActionResult AssociateEventWithAppUser(int EventId, int UserId)
        {

            Event SelectedEvent = db.Events.Include(e => e.AppUsers).FirstOrDefault(e => e.EventId == EventId);
            AppUser SelectedAppUser = db.AppUsers.Find(UserId);

            if (SelectedEvent == null || SelectedAppUser == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input EventId  is: " + EventId);
            Debug.WriteLine("selected Event Title is: " + SelectedEvent.Title);
            Debug.WriteLine("input UserId is: " + UserId);
            Debug.WriteLine("selected UserName is: " + SelectedAppUser.UserName);

            //SQL equivalent:
            //insert into EventAppUsers (EventId,UserId) values ({EventId}/{UserId})

            SelectedEvent.AppUsers.Add(SelectedAppUser);
            db.SaveChanges();

            return Ok();
        }
        //UnAssociateEventWithAppUser
        //api/eventData/UnAssociateEventWithAppUser/1/3

        [HttpPost]
        [Route("api/EventData/UnAssociateEventWithAppUser/{EventId}/{UserId}")]
        public IHttpActionResult UnAssociateEventWithAppUser(int EventId, int UserId)
        {

            Event SelectedEvent = db.Events.Include(e => e.AppUsers).FirstOrDefault(e => e.EventId == EventId);
            AppUser SelectedAppUser = db.AppUsers.Find(UserId);

            if (SelectedEvent == null || SelectedAppUser == null)
            {
                return NotFound();
            }


            SelectedEvent.AppUsers.Remove(SelectedAppUser);
            db.SaveChanges();

            return Ok();
        }

        //FindEvent
        // GET: api/EventData/FindEvent/2
        [ResponseType(typeof(EventDto))]
        [HttpGet]
        public IHttpActionResult FindEvent(int id)
        {
            Event Event = db.Events.Find(id);
            EventDto EventDto = new EventDto()
            {
                EventId = Event.EventId,
                Title = Event.Title,
                Location = Event.Location,
                EventDateTime = Event.EventDateTime,
                Capacity = Event.Capacity,
                Details = Event.Details,
                CategoryId = Event.Category.CategoryId,
                CategoryName = Event.Category.CategoryName

            };
            if (Event == null)
            {
                return NotFound();
            }

            return Ok(EventDto);
        }


        //AddEvent
        // POST: api/EventData/AddEvent
        [ResponseType(typeof(Event))]
        [HttpPost]
        public IHttpActionResult AddEvent(Event newEvent)
        {  
            Debug.WriteLine("I have reached the add event method!");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            db.Events.Add(newEvent);

            db.SaveChanges();

            return CreatedAtRoute("DefaultApi",null, newEvent);
        }

        /// <summary>
        /// Handle image upload and save it to image folder
        /// </summary>
        /// <param name="id"> Event id</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UploadEventImage(int id)
        {
            //bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var eventImage = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (eventImage.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(eventImage.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/animals/{id}.{extension}
                                string folderPath = HttpContext.Current.Server.MapPath("~/Content/Images/Events/");
                                if (!Directory.Exists(folderPath))
                                {
                                    Directory.CreateDirectory(folderPath);
                                }
                                string path = Path.Combine(folderPath, fn);
                                
                                //save the file
                                eventImage.SaveAs(path);

                                //if these are all successful then we can set these fields
                                //haspic = true;
                                picextension = extension;

                                //Update the animal haspic and picextension fields in the database
                                Event SelectedEvent = db.Events.Find(id);
                                SelectedEvent.ImagePath = "/Content/Images/Events/" + fn;
                                //SelectedEvent.AnimalHasPic = haspic;
                                //SelectedEvent.PicExtension = extension;
                                db.Entry(SelectedEvent).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Event Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }
        }



            // UpdateEvent
            // POST: api/EventData/UpdateEvent/9
            //CLI command:curl -H "Content-Type:application/json" -d @newEvent.json https://localhost:44317/api/EventData/UpdateEvent/9
            [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateEvent(int id, Event updatedEvent)
        {
            Debug.WriteLine("I have reached the update event method!");

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            Event existingEvent = db.Events.Find(id);

            if (existingEvent == null)
            {
                Debug.WriteLine("Event not found");
                return NotFound();
            }

            // Only update properties user want to update
            existingEvent.UpdateDate = updatedEvent.UpdateDate != default
            ? updatedEvent.UpdateDate
            : existingEvent.UpdateDate;//I leart that this patten is a short version of if statement. The patten A?B:C means if A then B , else C;
                                       // The update datetime should be current datetime, however, since it is a MVP ,I just leave it as original one for now
            existingEvent.Title = updatedEvent.Title ?? existingEvent.Title;//I learnt that this patten is a short version of another if statement. The patten A=B??C means If B is not null, let A=B, otherwise use A=C
            existingEvent.Location = updatedEvent.Location ?? existingEvent.Location;
            existingEvent.EventDateTime = updatedEvent.EventDateTime != default
            ? updatedEvent.EventDateTime
            : existingEvent.EventDateTime;
            existingEvent.Capacity = updatedEvent.Capacity ?? existingEvent.Capacity;
            existingEvent.Details = updatedEvent.Details ?? existingEvent.Details;
         
            existingEvent.CategoryId = updatedEvent.CategoryId;


            try
            {
                db.SaveChanges();
                Debug.WriteLine("Event updated successfully");
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    Debug.WriteLine("Event not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        //DeleteEvent
        // POST: api/EventData/DeleteEvent/14
        [ResponseType(typeof(Event))]
        [HttpPost]
        public IHttpActionResult DeleteEvent(int id)
        {
            Event existingEvent = db.Events.Find(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            db.Events.Remove(existingEvent);
            db.SaveChanges();

            return Ok();
        }

        private bool EventExists(int id)
        {
            return db.Events.Count(e => e.EventId == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}




//Releted methods include:
//ListEventForCategory
//ListEventForUser
//AddEventToUser
//RemoveEventFromUser



