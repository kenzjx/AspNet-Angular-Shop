using Application.Abtractions;
using Application.Features.Notifications.Models;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomer;

public class CustomerCreated : INotification
{
    public string CustomerId { get; set; }

    public class CustomerCreatedHandler : INotificationHandler<CustomerCreated>
    {
        private readonly IEmailService _email;

        public CustomerCreatedHandler(IEmailService email)
        {
            _email = email;
        }

        public async Task Handle(CustomerCreated notification, CancellationToken cancellationToken)
        {
            await _email.SendCustomerCreatedEmail(new EmailMessage());
        }
    }
}