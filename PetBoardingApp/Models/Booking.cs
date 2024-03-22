using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetBoardingApp.Models { 
    public class Booking
    {
        [Key]
        public Guid BookingID { get; set; }

        [Required]
        public DateTime BookingStartTime { get; set; }

        [Required]
        public DateTime BookingEndTime { get; set; }

        [Required]
        [ForeignKey("PetID")]
        public virtual Pet Pet { get; set; }

        public Guid PetID { get; set; }

        public DateTime? TimeCheckedIn { get; set; }

        public DateTime?  TimeCheckedOut { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [MaxLength(100)]
        public string Status { get; set; }

        public Booking()
        {
            BookingID = Guid.NewGuid();
        }
    }
}