//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HappyHotels.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HotelAmenity
    {
        public int hotelamenities_id { get; set; }
        public int hotel_id { get; set; }
        public int amenity_id { get; set; }
    
        public virtual Amenity Amenity { get; set; }
        public virtual Hotel Hotel { get; set; }
    }
}
