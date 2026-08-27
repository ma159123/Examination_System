using Microsoft.AspNetCore.Http;

namespace Domain.Common.Errors;

public record Error(string Code, string Description, int? StatusCode = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified value is null.", StatusCodes.Status400BadRequest);

    public static readonly Error PageOutOfRange = new(
        "Pagination.PageOutOfRange",
        "The requested page number is out of range.",
        StatusCodes.Status400BadRequest
    );

    public static readonly Error DbError = new(
        "Database.Error",
        "An error occurred while accessing the database.",
        StatusCodes.Status500InternalServerError
    );

    // تم تغيير الـ Status Code إلى 409 Conflict
    public static readonly Error AlreadyEmailExist = new(
        "Found.AlreadyEmailExist",
        "Email already registered.",
        StatusCodes.Status409Conflict
    );

    public static readonly Error NotFound = new(
        "Found.NotFound",
        "Requested element was not found.",
        StatusCodes.Status404NotFound
    );
    public static readonly Error ResendLimitExceeded = new(
          "Auth.ResendLimitExceeded",
                "Too many OTP requests. You can only request up to 3 OTPs per hour.",
              StatusCodes.Status429TooManyRequests);

    public static readonly Error UserNotFound = new(
    "Found.UserNotFound",
    "UserNotFound not found.",
    StatusCodes.Status404NotFound
);
    public static readonly Error UserAlreadyAuthenticated = new(
   "Found.UserAlreadyAuthenticated",
   "User is already authenticated.",
   StatusCodes.Status400BadRequest
);

    public static readonly Error AlreadyEmailConfirmed = new(
        "Confirmation.AlreadyEmailConfirmed",
        "Email is already confirmed.",
        StatusCodes.Status400BadRequest
    );


    public static readonly Error EmailNotConfirmed = new(
        "Confirmation.EmailNotConfirmed",
        "Email is not confirmed.",
        StatusCodes.Status400BadRequest
    );

    public static readonly Error TooManyRequests = new(
        "Auth.TooManyRequests",
        "Too many requests.",
        StatusCodes.Status429TooManyRequests // تم تغيير الـ Status Code لـ 429
    );

    public static readonly Error Unauthorized = new(
        "Auth.Unauthorized",
        "User not authenticated.",
        StatusCodes.Status401Unauthorized
    );
    //rn 422 Unprocessable Entity with fiel
    public static readonly Error validationError = new(
       "Auth.validationError",
       "Field-level validation failed.",
       StatusCodes.Status422UnprocessableEntity
   );
    public static readonly Error InValidTokenError = new(
     "Auth.InValidTokenError",
     "Invalid token provided.",
     StatusCodes.Status422UnprocessableEntity
 );
    //rn 422 with specific password policy viol
    public static readonly Error PasswordPolicyViolation = new(
    "Auth.PasswordPolicyViolation",
    "Password does not meet the required policy.",
    StatusCodes.Status422UnprocessableEntity
);
}