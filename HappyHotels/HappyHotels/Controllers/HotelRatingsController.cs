using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HappyHotels.Models;
using Microsoft.AspNet.Identity;

namespace HappyHotels.Controllers
{
    public class HotelRatingsController : Controller
    {
        private HappyHotelsEntities db = new HappyHotelsEntities();

        // GET: HotelRatings
        public ActionResult Index()
        {
            var hotelRatings = db.HotelRatings.Include(h => h.Hotel);
            return View(hotelRatings.ToList());
        }

        // GET: HotelRatings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelRating hotelRating = db.HotelRatings.Find(id);
            if (hotelRating == null)
            {
                return HttpNotFound();
            }
            return View(hotelRating);
        }

        // GET: HotelRatings/Create
        public ActionResult Create(int hotelID)
        {
            var hotels = db.Hotels.ToList();
            if (hotelID != 0)
            {
                hotels = db.Hotels.Where(h => h.hotel_id == hotelID).ToList();
            }
            ViewBag.hotel_id = new SelectList(hotels, "hotel_id", "name");
            return View();
        }

        // POST: HotelRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "hotelrating_id,hotel_id,rating,review,user_id")] HotelRating hotelRating)
        {
            hotelRating.user_id = User.Identity.GetUserId();
            if (ModelState.IsValid && hotelRating.rating > 0)
            {
                db.HotelRatings.Add(hotelRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.hotel_id = new SelectList(db.Hotels, "hotel_id", "name", hotelRating.hotel_id);
            return View(hotelRating);
        }

        // GET: HotelRatings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelRating hotelRating = db.HotelRatings.Find(id);
            if (hotelRating == null)
            {
                return HttpNotFound();
            }
            ViewBag.hotel_id = new SelectList(db.Hotels, "hotel_id", "name", hotelRating.hotel_id);
            return View(hotelRating);
        }

        // POST: HotelRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "hotelrating_id,hotel_id,rating,review,user_id")] HotelRating hotelRating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotelRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.hotel_id = new SelectList(db.Hotels, "hotel_id", "name", hotelRating.hotel_id);
            return View(hotelRating);
        }

        // GET: HotelRatings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelRating hotelRating = db.HotelRatings.Find(id);
            if (hotelRating == null)
            {
                return HttpNotFound();
            }
            return View(hotelRating);
        }

        // POST: HotelRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HotelRating hotelRating = db.HotelRatings.Find(id);
            db.HotelRatings.Remove(hotelRating);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
