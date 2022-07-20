using Application.Features.Notifications.Models;

namespace Application.Abtractions;

public interface IEmailService
{
    Task RegistrationConfirmationEmail(string to, string link);
    
    Task ForgottentPasswordEmail(string to, string link);
     Task SendCustomerCreatedEmail(EmailMessage emailMessage);
   
    
}