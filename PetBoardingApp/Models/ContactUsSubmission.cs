using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetBoardingApp.Models
{
    public class ContactUsSubmission
    {
        [Key]
        public Guid ContactUsSubmissionID { get; set; }

        [Required]
        public string Email { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        public ContactUsSubmission()
        {
            ContactUsSubmissionID = Guid.NewGuid();
        }
    }
}