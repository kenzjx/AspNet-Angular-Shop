using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Application.Abtractions;
using Application.Features.Notifications.Models;
using Application.Settings;
using Microsoft.Extensions.Logging;

namespace ClassLibrary1.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailSettings emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public Task RegistrationConfirmationEmail(string to, string link)
    {
        var registrationTemplate = GetBaseTemplate("RegistrationTemplate");

        registrationTemplate = registrationTemplate.Replace("@user@", to)
            .Replace("@linktext@", "Activate account")
            .Replace("@link@", link);

        var emailMessage = new EmailMessage()
        {
            To = to,
            Body = registrationTemplate,
            Subject = "Confirm you resgistration",
            From = _emailSettings.SmtpSenderAddress
        };

        SendAsync(emailMessage);
        return Task.CompletedTask;
    }

    private Task SendAsync(EmailMessage emailMessage)
    {
        try
        {
            var mailMessage = new MailMessage(_emailSettings.SmtpSenderAddress, emailMessage.To, emailMessage.Subject,
                emailMessage.Body)
            {
                From = new MailAddress(_emailSettings.SmtpSenderAddress, _emailSettings.SmtpSenderName),
                IsBodyHtml = true
            };

            foreach (var attachement in emailMessage.Attachments)
            {
                mailMessage.Attachments.Add(
                    new Attachment(
                        new MemoryStream(attachement.Content), attachement.Name, attachement.ContentType));
            }

            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
            };
            
            client.Send(mailMessage);

            _logger.LogInformation($"Email send successfully to: {emailMessage.To}. Subject: {emailMessage.Subject}");
            
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError($"Email failed to send to {emailMessage.To}", e);
            throw;
        }
    }

    private string GetBaseTemplate(string registrationtemplate)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var baseTemplateStream = assembly.GetManifestResourceStream("Infrastructure.Email.Templates.BaseTemplate.html");
        using var baseReader = new StreamReader(baseTemplateStream, Encoding.UTF8);
        var baseTemplate = baseReader.ReadToEnd();

        var bodyTempalteStream =
            assembly.GetManifestResourceStream($"Infrastructure.Email.Templates.{registrationtemplate}.html");
        using var bodyReader = new StreamReader(bodyTempalteStream, Encoding.UTF8);
        var bodyTemaplte = bodyReader.ReadToEnd();

        var template = baseTemplate.Replace("@content@", bodyTemaplte);

        return template;
    }

    public Task ForgottentPasswordEmail(string to, string link)
    {
        var registrationTemplate = GetBaseTemplate("RegistrationTemplate");
        registrationTemplate = registrationTemplate.Replace("@user@", to)
            .Replace("@linktext@", "Reset password")
            .Replace("@link@", link);
        var emailMessage = new EmailMessage
        {
            To = to,
            Body = registrationTemplate,
            Subject = "Reset password",
            From = _emailSettings.SmtpSenderAddress
        };
        SendAsync(emailMessage);
        return Task.CompletedTask;
    }

    public Task SendCustomerCreatedEmail(EmailMessage emailMessage)
    {
        throw new NotImplementedException();
    }
}