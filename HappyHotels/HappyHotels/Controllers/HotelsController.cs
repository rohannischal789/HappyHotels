﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HappyHotels.Models;

namespace HappyHotels.Controllers
{
    public class HotelsController : Controller
    {
        private HappyHotelsEntities db = new HappyHotelsEntities();

        //[Authorize]
        //// GET: Hotels
        //public ActionResult Index()
        //{
        //    return View(db.Hotels.ToList());
        //}

        // GET: Hotels
        public ActionResult Index(String destination)
        {
            if (destination != null && destination.Trim() != "")
            {
                return View(db.Hotels.Where(h => h.address.Contains(destination) || h.city.Contains(destination)).ToList());
            }
            else
            {
                return View(db.Hotels.ToList());
            }
        }

        // GET: Hotels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        [Authorize]
        // GET: Hotels/Create
        public ActionResult Create()
        {
            return View();
        }
        [Authorize]
        // POST: Hotels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "hotel_id,name,description,address,city,state,country,postcode,check_in_time,check_out_time,lattitude,longitude,rating")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                db.Hotels.Add(hotel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel);
        }

        // GET: Hotels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "hotel_id,name,description,address,city,state,country,postcode,check_in_time,check_out_time,lattitude,longitude,rating")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel);
        }
        [Authorize]
        // GET: Hotels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hotel hotel = db.Hotels.Find(id);
            db.Hotels.Remove(hotel);
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

        public ActionResult Dashboard(int? hotelID)
        {
            List<int> ratingCount = new List<int>();
            List<decimal> ratings = new List<decimal>();
            if (hotelID != null && hotelID != 0)
            {
                ratings = db.HotelRatings.Where(h => h.hotel_id == hotelID).Select(r => r.rating).Distinct().OrderBy(r => r).ToList();
            }
            else
            {
                ratings = db.HotelRatings.Select(r => r.rating).Distinct().OrderBy(r => r).ToList();
            }
            foreach (var rating in ratings)
            {
                ratingCount.Add(db.HotelRatings.Count(r => r.rating == rating));
            }
            ViewBag.ratings = ratings;
            ViewBag.ratingCount = ratingCount;
            return View();
        }
    }
}
