using System;
using System.Net;
using System.Net.Mail;

public class GenerateMail
{
    private string _smtpServer = "smtp.gmail.com";
    private int _port = 587;
    private string _senderEmail = "mishraayush75584@gmail.com";  
    private string _password = "wqcwovgespobpahw"; 


    public async Task SendEmailAsync(string recipientEmail, string subject, string htmlBody)
    {
        try
        {
            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _port;
                smtpClient.Credentials = new NetworkCredential(_senderEmail, _password);
                smtpClient.EnableSsl = true;

                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                
                mailMessage.To.Add(recipientEmail);

                
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent to {recipientEmail} with subject: {subject}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email. Error: {ex.Message}");
        }
    }
}
