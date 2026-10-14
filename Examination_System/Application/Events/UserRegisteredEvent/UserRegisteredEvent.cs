using MediatR;

namespace Application.Events.UserRegisteredEvent;

public record UserRegisteredEvent(string UserId, string Email) : INotification;
