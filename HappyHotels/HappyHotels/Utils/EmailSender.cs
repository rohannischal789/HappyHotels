using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace HappyHotels.Utils
{
    public class EmailSender
    {
        private const String API_KEY = "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";

        public void Send(String toEmail, String subject, String contents)
        {
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("noreply@localhost.com", "Happy Hotels Support User");
            var to = new EmailAddress(toEmail, "");
            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }
    }
}