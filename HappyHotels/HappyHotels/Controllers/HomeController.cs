using HappyHotels.Models;
using HappyHotels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HappyHotels.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult EmailView()
        {
            var model = new EmailViewModel();
            model.EmailAddresses = new List<SelectListItem>();
            var context = new ApplicationDbContext();
            var customerRole = context.Roles.FirstOrDefault(r => r.Name == "CUSTOMER");
            var customers = context.Users.Where(u=>u.Roles.Any(r=> r.RoleId == customerRole.Id)).ToList();
            foreach (var customer in customers)
            {
                model.EmailAddresses.Add(new SelectListItem() { Text = customer.Email, Value = customer.Email, Selected = false });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EmailView(EmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    String toEmail = model.ToEmail;
                    String subject = model.Subject;
                    String contents = model.Contents;

                    EmailSender es = new EmailSender();
                    es.Send(toEmail, subject, contents);

                    ViewBag.Result = "Email has been sent.";

                    ModelState.Clear();

                    return View(new EmailViewModel());
                }
                catch
                {
                    return View();
                }
            }

            return View();
        }
    }
}