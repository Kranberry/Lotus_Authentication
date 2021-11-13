namespace Lotus_Authentication.Models;

public record Country(int Id, string Name, string NiceName, string Iso2, string? Iso3, int? NumCode, int? PhoneCode);
