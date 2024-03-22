using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework.Constraints;
using PetBoardingApp.Models;

namespace PetBoardingApp.ViewModels
{

    public class BookingVM
    {
        [Required]
        [MaxLength(100)]
        public string StartDate { get; set; }

        [MaxLength(100)]
        public string EndDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string PetID { get; set; }

        [MaxLength(100)]
        public string StartTime { get; set; }

        [MaxLength(100)]
        public string EndTime { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }

    // Helper class - need bookings index to use a model that contains a list of user bookings, list of user pets, and a booking view model
    public class BookingIndexVM 
    {
        public string OPENING_TIME = GlobalConstants.OPENING_TIME;
        public string CLOSING_TIME = GlobalConstants.CLOSING_TIME;

        public List<SelectListItem> TimesList { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<Pet> Pets { get; set; }
        public BookingVM BookingVM { get; set; }

        public BookingIndexVM()
        {
            TimesList = new List<SelectListItem>();
            DateTime openingTime = DateTime.Parse(OPENING_TIME);
            DateTime closingTime = DateTime.Parse(CLOSING_TIME);
            DateTime currentTime = openingTime;
            while (currentTime <= closingTime)
            {
                TimesList.Add(new SelectListItem { Text = currentTime.ToShortTimeString(), Value = currentTime.ToShortTimeString() });
                currentTime = currentTime.AddMinutes(60);
            }
        }
    }
}