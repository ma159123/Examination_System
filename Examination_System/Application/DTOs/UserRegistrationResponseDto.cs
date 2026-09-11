namespace Application.DTOs;

public record UserRegistrationResponseDto(
string UserId,
string Email,
string FullName,
string AccountStatus,
DateTime CreatedAt
);
