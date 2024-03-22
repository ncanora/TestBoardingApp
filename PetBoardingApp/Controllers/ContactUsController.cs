using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PetBoardingApp.Models;
using PetBoardingApp.ViewModels;
using PetBoardingApp.Views;
using PetBoardingApp.Helpers;
using PetBoardingApp.App_Start;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNet.Identity.Owin;
namespace PetBoardingApp.Views.ContactUs
{
    public class ContactUsController : Controller
    {
        // GET: ContactUsVM
        public ActionResult Index()
        {   
            return View(new ContactUsSubmissionVM());
        }

        // POST: ContactUs
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateGoogleCaptcha]
        public async Task<ActionResult> Index(ContactUsSubmissionVM contactUsSubmissionVM)
        {
            if (User.Identity.IsAuthenticated)
            {

                var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = await manager.FindByIdAsync(User.Identity.GetUserId());
                contactUsSubmissionVM.Email = user.GetEmail();
                contactUsSubmissionVM.FirstName = user.FirstName;
                contactUsSubmissionVM.LastName = user.LastName;
            }
            if (contactUsSubmissionVM.FirstName.IsNullOrWhiteSpace() || contactUsSubmissionVM.LastName.IsNullOrWhiteSpace() || contactUsSubmissionVM.Email.IsNullOrWhiteSpace() || contactUsSubmissionVM.Message.IsNullOrWhiteSpace())
            {
                contactUsSubmissionVM.Errors = "Please fill out all fields!";
                return View(contactUsSubmissionVM);
            }
            ApplicationDbContext db = new ApplicationDbContext();
            ContactUsSubmission contactUsSubmission = new ContactUsSubmission();

            // map the properties from the view model to the model
            foreach (var property in contactUsSubmissionVM.GetType().GetProperties())
            {
                if (property.Name == "Errors")
                    continue;
                contactUsSubmission.GetType().GetProperty(property.Name).SetValue(contactUsSubmission, property.GetValue(contactUsSubmissionVM));
            }

            db.ContactUsSubmissions.Add(contactUsSubmission);
            try
            {
                var emailService = new EmailService();
                var identityMessage = new IdentityMessage
                {
                    Destination = "ncanora860@gmail.com",
                    Subject = $"Message from {contactUsSubmission.FirstName} {contactUsSubmission.LastName} via Test Boarding App",
                    Body = $"Email: {contactUsSubmission.Email}\nMessage: {contactUsSubmission.Message}"
                };

                await emailService.SendAsync(identityMessage);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(contactUsSubmission);
                Console.WriteLine(e);
                contactUsSubmissionVM.Errors = "Sorry! Something went wrong. Please try again.";
                return View(contactUsSubmissionVM);
            }   
            return View("MessageSentSuccess");
        }
    }
}