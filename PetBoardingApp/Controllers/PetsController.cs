using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetBoardingApp.Models;
using PetBoardingApp.ViewModels;

namespace PetBoardingApp.Views.Pets
{
    [Authorize]
    public class PetsController : Controller
    {
        // GET: Pets
        public ActionResult Index()
        {
            return View(GetPets());
        }

        // returns the view for adding a pet]
        public ActionResult Add()
        {
            return View(new PetVM());
        }

        // parses the form data and adds a pet to the database
        [HttpPost]
        public ActionResult Add(PetVM petVM)
        {

            ApplicationDbContext db = new ApplicationDbContext();

            Pet pet = new Pet();
            pet.Name = petVM.Name;
            pet.Breed = petVM.Breed;
            pet.Age = petVM.Age;
            pet.Notes = petVM.Notes;

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(currentUserId);
            pet.Owner = user;

            db.Pets.Add(pet);

            try
            {
                db.SaveChanges();
                return View("Success");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult Remove(Guid id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Pet pet = db.Pets.Find(id);
            List<Booking> bookings = GetPetBookings(pet, db);

            if (pet == null)
                return Content("Pet not found");

            // Remove appointments associated with pet
            foreach (Booking booking in bookings)
            {
                db.Bookings.Attach(booking);
                db.Bookings.Remove(booking);
            }
            db.Pets.Remove(pet);

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View("Error");
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Pet pet = db.Pets.Find(id);
            PetVM petVM = new PetVM();

            petVM.Name = pet.Name;
            petVM.Breed = pet.Breed;
            petVM.Age = pet.Age;
            petVM.Notes = pet.Notes;

            return View(petVM);

        }
        [HttpPost]
        public ActionResult Edit(Guid id, PetVM petVM)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Pet pet = db.Pets.Find(id);

            if (pet == null)
            {
                return Content("Pet not found");
            }
            pet.Name = petVM.Name;
            pet.Breed = petVM.Breed;
            pet.Age = petVM.Age;
            pet.Notes = petVM.Notes;

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }   
            return View("Error");
        }

        // returns a list of pets for the current user
        [HttpGet]
        public List<Pet> GetPets()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(currentUserId);

            List<Pet> pets = db.Pets.Where(p => p.Owner.Id == user.Id).ToList();

            return pets;
        }

        [HttpGet]
        public static List<Booking> GetPetBookings(Pet pet, ApplicationDbContext applicationDbContext)
        {
            if (applicationDbContext == null)
            {
                applicationDbContext = new ApplicationDbContext();
            }

            List<Booking> bookings = applicationDbContext.Bookings.Where(b => b.Pet.PetID == pet.PetID).ToList();

            return bookings;
        }
    }
}