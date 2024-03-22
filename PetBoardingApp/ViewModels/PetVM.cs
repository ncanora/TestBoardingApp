using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetBoardingApp.ViewModels
{
    public class PetVM
    {

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Breed { get; set; }

        public int? Age { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

    }
}