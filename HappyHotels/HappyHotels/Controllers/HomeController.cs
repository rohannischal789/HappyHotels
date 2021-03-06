﻿using HappyHotels.Models;
using HappyHotels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HappyHotels.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult EmailView()
        {
            if (User.IsInRole("ADMIN"))
            {
                var model = new SendEmailModel();
                ViewBag.AllEmailAddresses = GetEmails();
                return View(model);
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
            }
        }

        private MultiSelectList GetEmails()
        {
            using (var context = new ApplicationDbContext())
            {
                var customerRole = context.Roles.FirstOrDefault(r => r.Name == "CUSTOMER");
                var customers = context.Users.Where(u => u.Roles.Any(r => r.RoleId == customerRole.Id)).ToList(); // get all users who are customers
                var data = context.Users.Where(u => u.Roles.Any(r => r.RoleId == customerRole.Id)).Select(c => new
                {
                    Name = c.Email,
                    Value = c.Email
                }).ToList();
                return new MultiSelectList(data, "Name", "Value");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult EmailView(SendEmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var toEmail = model.SelectedEmailAddresses.ToList();
                    String subject = model.Subject;
                    String contents = model.Contents;

                    EmailSender es = new EmailSender();
                    es.Send(toEmail, subject, contents, model.Upload);

                    ViewBag.Result = "Email has been sent.";

                    ModelState.Clear();
                    model.Contents = "";
                    model.Subject = "";
                    ViewBag.AllEmailAddresses = GetEmails();
                    return View(model);
                }
                catch
                {
                    ViewBag.AllEmailAddresses = GetEmails();
                    return View();
                }
            }
            ViewBag.AllEmailAddresses = GetEmails();
            return View();
        }
    }
}