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
    
    public partial class Landmark
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Landmark()
        {
            this.HotelLandmarks = new HashSet<HotelLandmark>();
        }
    
        public int landmark_id { get; set; }
        public string landmark_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelLandmark> HotelLandmarks { get; set; }
    }
}
