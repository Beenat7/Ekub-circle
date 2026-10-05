namespace Ekub.Application.Auth.DTOs;

public sealed record LoginRequest(
    string Phonenumber,
    string Password
);