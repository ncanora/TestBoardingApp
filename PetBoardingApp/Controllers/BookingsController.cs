using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PetBoardingApp.Models;
using System.Data.Entity;
using PetBoardingApp.ViewModels;
using System.Globalization;

namespace PetBoardingApp.Views.Bookings
{
    public class BookingsController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            BookingIndexVM bookingIndexVM = new BookingIndexVM();
            bookingIndexVM.Bookings = this.GetBookings();
            bookingIndexVM.Pets = this.GetPets();
            BookingVM bookingVM= TempData["BookingVM"] as BookingVM;
            string errorMessage = TempData["ErrorMessage"] as string;

            // Default view (no form submission)
            if (bookingVM == null || errorMessage == null)
            {
                bookingIndexVM = new BookingIndexVM();
                bookingIndexVM.Bookings = this.GetBookings();
                bookingIndexVM.Pets = this.GetPets();
                return View(bookingIndexVM);
            }

            // If form submission was invalid, show modal with error message
            bookingIndexVM.BookingVM = bookingVM; // Pass the invalid BookingVM back to the view
            ViewBag.ShowModal = true;
            ViewBag.ErrorMessage = errorMessage;
            return View("Index", bookingIndexVM);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(BookingVM bookingVM)
        {

            // Check if the model is valid
            if (this.GetBookings().Count > 50)
            {
                TempData["BookingVM"] = bookingVM;
                TempData["ErrorMessage"] = "You have reached the maximum number of bookings";
                return RedirectToAction("Index");
            }
            else if (!ModelState.IsValid)
            {
                TempData["BookingVM"] = bookingVM;
                TempData["ErrorMessage"] = "Missing or invalid fields";
                return RedirectToAction("Index");

            }

            // Default start and end times
            if (bookingVM.StartTime == null)
                bookingVM.StartTime = "7:00 AM";

            if (bookingVM.EndTime == null)
                bookingVM.EndTime = "8:00 PM";

            if (bookingVM.EndDate == null)
                bookingVM.EndDate = bookingVM.StartDate;

            ApplicationDbContext db = new ApplicationDbContext();
            List<Booking> currentSamePetBookings = this.GetBookings().Where(b => b.Status != "CANCELLED" && b.Pet.PetID == Guid.Parse(bookingVM.PetID)).ToList();
            Booking booking = new Booking();
            DateTime parsedStart, parsedEnd;

            bool isStartParsed = DateTime.TryParseExact($"{bookingVM.StartDate} {bookingVM.StartTime}", "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStart);
            bool isEndParsed = DateTime.TryParseExact($"{bookingVM.EndDate} {bookingVM.EndTime}", "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEnd);


            // Error handling for invalid date(s)
            if (!isStartParsed || !isEndParsed)
            {
                TempData["BookingVM"] = bookingVM;
                TempData["ErrorMessage"] = "Invalid date format";
                return RedirectToAction("Index");
            }

            if (parsedEnd <= parsedStart)
            {
                TempData["BookingVM"] = bookingVM;
                TempData["ErrorMessage"] = $"{parsedStart} to {parsedEnd} is an invalid booking time";
                return RedirectToAction("Index");

            }
            if (parsedStart.Date == DateTime.Now.Date)
            {
                TempData["BookingVM"] = bookingVM;
                TempData["ErrorMessage"] = "You cannot schedule same-day appointments";
                return RedirectToAction("Index");
            }

            if (parsedStart < DateTime.Now)
            {
                TempData["BookingVM"] = bookingVM;
                TempData["ErrorMessage"] = "You cannot book a time in the past";
                return RedirectToAction("Index");
            }

            if ((parsedEnd - parsedStart).TotalDays > 8)
            { TempData["BookingVM"] = bookingVM;
              TempData["ErrorMessage"] = "You cannot book a time more than 1 week (from date) at a time";
              return RedirectToAction("Index");
            }

            // For unique pet, check if there are any overlapping bookings or same date booking
            bool existingBookingsInside = currentSamePetBookings.Any(b => b.BookingStartTime.Date <= parsedStart.Date && b.BookingEndTime.Date >= parsedEnd.Date);
            bool existingBookingsOutside = currentSamePetBookings.Any(b => b.BookingStartTime.Date >= parsedStart.Date && b.BookingEndTime.Date <= parsedEnd.Date);
            bool existingBookingsOverlap = currentSamePetBookings.Any(b => b.BookingStartTime.Date <= parsedStart.Date && b.BookingEndTime.Date >= parsedStart.Date) || currentSamePetBookings.Any(b => b.BookingStartTime.Date <= parsedEnd.Date && b.BookingEndTime.Date >= parsedEnd.Date);
            
            if (existingBookingsInside || existingBookingsOutside || existingBookingsOverlap)
            {
                TempData["BookingVM"] = bookingVM;
                TempData["ErrorMessage"] = "This pet already has a appointment at this time or within the time slot";
                return RedirectToAction("Index");
            }
            // Create booking
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            booking.BookingStartTime = parsedStart;
            booking.BookingEndTime = parsedEnd;
            booking.Pet = db.Pets.Find(Guid.Parse(bookingVM.PetID));
            booking.Name = currentUser.FirstName + " " + currentUser.LastName;
            booking.Notes = bookingVM.Notes;
            booking.Status = "APPROVED";

            try
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View("Error");

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Guid id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Booking booking = db.Bookings.Find(id);
            booking.Status = "CANCELLED";

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
        
        // Get the list of bookings for the current user
        [Authorize]
        [HttpGet]
        public List<Booking> GetBookings()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(currentUserId);


            // Get the list of owned pets
            List<Guid> pets = db.Pets
                .Where(p => p.Owner.Id == user.Id)
                .Select (p => p.PetID)
                .ToList();

            List<Booking> bookings = db.Bookings
                .Include(b => b.Pet)
                .Where(b => pets.Contains(b.Pet.PetID) && b.Status != "CANCELLED")
                .ToList();

            return bookings;
        }


        [Authorize]
        [HttpGet]
        public List<Pet> GetPets()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(currentUserId);

            List<Pet> pets = db.Pets
                .Where(p => p.Owner.Id == user.Id)
                .ToList();

            return pets;
        }
    }
}