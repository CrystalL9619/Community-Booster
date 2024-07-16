using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPassionProject.Models.ViewModels
{
    public class FindEvent
    {
        //This is a class
        public EventDto SelectedEvent { get; set; }

        public string CurrentUserId { get; set; }
        public IEnumerable<EventDto> RelatedEvents { get; set; }
        public string CreatorId { get; set; }     

        public IEnumerable<EventDto> CreatedEvents { get; set; }

        public IEnumerable<EventDto> SavedEvents { get; set; }

        public bool isSaved { get; set; }

        public string ActiveTab { get; set; }
        //public IEnumerable<AppUserDto> ParticipatingUsers { get; set; }

        //public IEnumerable<AppUserDto> NotPaticipatingUsers { get; set; }

    }
}