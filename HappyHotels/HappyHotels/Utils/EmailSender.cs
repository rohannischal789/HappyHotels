using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace HappyHotels.Utils
{
    public class EmailSender
    {
        private const String API_KEY = "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";

        public void Send(List<string> toEmails, String subject, String contents, HttpPostedFileBase attachment)
        {
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("noreply@localhost.com", "Happy Hotels Support User");
            List<EmailAddress> emailList = new List<EmailAddress>();
            foreach(var toEmail in toEmails)
            {
                emailList.Add(new EmailAddress(toEmail, "")); // convert to email address
            }
            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>";
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailList, subject, plainTextContent, htmlContent);
            //if (attachment != null && attachment.ContentLength > 0)
            //{
               // msg.Attachments.Add(new Attachment(attachment.InputStream,
               //System.IO.Path.GetFileName(attachment.FileName)));
            //}
            if (attachment != null && attachment.ContentLength > 0) // if attachment added
            {
                MemoryStream target = new MemoryStream();
                attachment.InputStream.CopyTo(target);
                byte[] byteData = target.ToArray(); // fetch byte data from the file
                msg.Attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Content = Convert.ToBase64String(byteData), // convert to base 64 for sending email
                        Filename = attachment.FileName,
                        Type = "txt/plain",
                        Disposition = "attachment"
                    }
                };
            }
            var response = client.SendEmailAsync(msg);
        }
    }
}