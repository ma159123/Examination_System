using Application.CQRS.Features.Otp.GenerateOtp;
using MediatR;

namespace Application.Events.UserRegisteredEvent
{
    public class SendOtpOnUserRegisteredHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IMediator _mediator;

        public SendOtpOnUserRegisteredHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            // execute the command to generate and send OTP
            await _mediator.Send(new GenerateAndSendOtpCommand(notification.UserId, notification.Email), cancellationToken);
        }
    }
}
