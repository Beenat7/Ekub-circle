namespace Ekub.Application.Auth.DTOs;

public sealed record LoginResponse(
    int Id,
    string Username,
    string FirstName,
    string MiddleName,
    string LastName,
    string Phonenumber,
    DateTime CreatedAt
);