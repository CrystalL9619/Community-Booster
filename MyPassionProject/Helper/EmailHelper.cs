using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Configuration;
using System.Diagnostics;

public static class EmailHelper
{
    public static void SendEmail(string toAddress, string subject, string body)
    {
        string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
        int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Crystal", smtpUsername)); // Display name and sender email
        message.To.Add(new MailboxAddress("liuhanzecrystal@gmail.com", toAddress)); // Recipient email

        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body
        };
        Debug.WriteLine("Email Sent");
        using (var client = new SmtpClient())
        {
            client.Connect(smtpServer, smtpPort, useSsl: true);
            client.Authenticate(smtpUsername, smtpPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
