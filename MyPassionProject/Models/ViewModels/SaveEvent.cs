using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyPassionProject.Models.ViewModels
{

        public class SavedEvent
        {
            [Key, Column(Order = 0)]
            public int Event_EventId { get; set; }

            [Key, Column(Order = 1)]
            public string ApplicationUser_Id { get; set; }

            // Navigation properties (if needed)
            [ForeignKey("Event_EventId")]
            public virtual Event Event { get; set; }

            [ForeignKey("ApplicationUser_Id")]
            public virtual ApplicationUser ApplicationUser { get; set; }
        }


    }