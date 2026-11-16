using Microsoft.AspNetCore.Identity;

namespace RazorPagesYacht.Services;

// Ukrainian texts for Identity errors (registration, password change)
public class UkrainianIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError() =>
        new() { Code = nameof(DefaultError), Description = "Сталася невідома помилка." };
    public override IdentityError DuplicateUserName(string userName) =>
        new() { Code = nameof(DuplicateUserName), Description = $"Користувач «{userName}» уже існує." };
    public override IdentityError DuplicateEmail(string email) =>
        new() { Code = nameof(DuplicateEmail), Description = $"Ел. пошта «{email}» уже використовується." };
    public override IdentityError InvalidEmail(string? email) =>
        new() { Code = nameof(InvalidEmail), Description = $"Некоректна ел. пошта «{email}»." };
    public override IdentityError InvalidUserName(string? userName) =>
        new() { Code = nameof(InvalidUserName), Description = $"Некоректне ім’я користувача «{userName}»." };
    public override IdentityError PasswordMismatch() =>
        new() { Code = nameof(PasswordMismatch), Description = "Неправильний пароль." };
    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = $"Пароль має містити щонайменше {length} символів." };
    public override IdentityError PasswordRequiresDigit() =>
        new() { Code = nameof(PasswordRequiresDigit), Description = "Пароль має містити хоча б одну цифру (0–9)." };
    public override IdentityError PasswordRequiresLower() =>
        new() { Code = nameof(PasswordRequiresLower), Description = "Пароль має містити хоча б одну малу літеру (a–z)." };
    public override IdentityError PasswordRequiresUpper() =>
        new() { Code = nameof(PasswordRequiresUpper), Description = "Пароль має містити хоча б одну велику літеру (A–Z)." };
    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Пароль має містити хоча б один спецсимвол (наприклад, # ! @)." };
    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"Пароль має містити щонайменше {uniqueChars} різних символів." };
}