using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HappyHotels.Models
{
    public class SendEmailModel
    {
        [Required(ErrorMessage = "Please select one or more email addresses")]
        public List<String> SelectedEmailAddresses{ get; set; }
        [Display(Name = "Email Addresses")]
        public MultiSelectList AllEmailAddresses { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter the contents")]
        public string Contents { get; set; }
        [Display(Name = "Attachment")]
        public HttpPostedFileBase Upload { get; set; }

    }
}