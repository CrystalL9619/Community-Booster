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
        public IEnumerable<AppUserDto> ParticipatingUsers { get; set; }

        public IEnumerable<AppUserDto> NotPaticipatingUsers { get; set; }
    }
}