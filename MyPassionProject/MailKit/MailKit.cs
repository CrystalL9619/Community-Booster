using System;
using System.Diagnostics;
using MailKit.Net.Smtp;
using MimeKit;

namespace MyPassionProject.MailKit
{
    public class MailKit
    {
        public void SendEmail()
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Community Booster", "communityboostertech@gmail.com"));
            message.To.Add(new MailboxAddress("Community Booster", "communityboostertech@gmail.com"));
            message.Subject = "Hello, World";
            message.Body = new TextPart("plain")
            {
                Text = @"We're here to serve you."
            };

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.Connect("smtp.gmail.com", 587, true); 
                    smtpClient.Authenticate("communityboostertech@gmail.com", "ddab jhsr ulul fqvq"); 
                    smtpClient.Send(message);
                    Debug.WriteLine("Email Sent");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    smtpClient.Disconnect(true);
                }
            }
        }
    }
}
