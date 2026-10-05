namespace Ekub.Application.Auth.DTOs;

public sealed record SignupRequest(
    string FirstName,
    string MiddleName,
    string LastName,
    string Username,
    string Password,
    string Phonenumber
);