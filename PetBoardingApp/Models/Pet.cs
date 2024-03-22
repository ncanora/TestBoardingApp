using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetBoardingApp.Models
{
    public class Pet
    {
        [Key]
        public Guid PetID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]    
        public string Breed { get; set; }

        public int? Age { get; set; }

        public string Notes { get; set; }

        [Required]
        public virtual ApplicationUser Owner { get; set; }

        public Pet()
        {
            PetID = Guid.NewGuid();
          }

    }
}