namespace Lotus_Authentication.Models;

public class User
{
    public int Id { get; init; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string Email { get; private set; }
    public string? Password { get; private set; }
    public byte[]? Salt { get; private set; }
    public string UserName { get; init; }
    public UserType UserType { get; init; }
    public Gender Gender { get; init; }
    public string CountryISO2 { get; private set; }
    public int? CountryCode { get; private set; }
    public int? CountryPhoneCode { get; private set; }
    public DateTime CreationDate { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
    public bool IsEmailConfirmed { get; init; }

    public User(int id, string? firstName, string? lastName, string email, string userName, UserType userType, Gender gender, string countryISO2, int? countryCode, int? countryPhoneCode, DateTime creationDate, DateTime? lastUpdatedDate, bool isEmailConfirmed)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = userName;
        UserType = userType;
        Gender = gender;
        CountryISO2 = countryISO2;
        CreationDate = creationDate;
        LastUpdatedDate = lastUpdatedDate;
        IsEmailConfirmed = isEmailConfirmed;
        CountryCode = countryCode;
        CountryPhoneCode = countryPhoneCode;
    }

    // System.Int32 user_id, System.String first_name, System.String last_name, System.String username, System.String email, System.String password, System.Byte[] salt, System.Int32 gender, System.Int32 fk_country_id, System.DateTime record_insert_date, System.DateTime record_update_date, System.Boolean is_validated) is required for Lotus_Authentication.Models.User 
    //public User(int user_id, string? first_name, string? last_name, string email, string username, string password, byte[] salt, Gender gender, int fk_country_id, DateTime creationDate, DateTime? lastUpdatedDate, bool isEmailConfirmed)

    public void SetPassword(string password) => Password = password;
    public void SetSalt(byte[] salt) => Salt = salt;

    public void SetCountry(int? countryCode, int? countryPhoneCode) { }
    public void SetCountry(string iso) { }

}