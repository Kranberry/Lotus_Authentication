using System.Reflection;
using System.Text.Json.Serialization;

namespace Lotus_Authentication.API.ApiModels;

public class ApiUserModel
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("new_password")]
    public string? NewPassword { get; set; }

    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    [JsonPropertyName("gender")]
    public Gender? Gender { get; set; }

    [JsonPropertyName("country_iso2")]
    public string? CountryISO2 { get; set; }

    [JsonPropertyName("country_code")]
    public int? CountryCode { get; set; }

    [JsonPropertyName("country_phone_code")]
    public int? CountryPhoneCode { get; set; }

    public ApiUserModel() { }

    public ApiUserModel(int? id, string? firstName, string? lastName, string? email, string? userName, Gender? gender, string? countryISO2, int? countryCode, int? countryPhoneCode)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = userName;
        Gender = gender;
        CountryISO2 = countryISO2;
        CountryCode = countryCode;
        CountryPhoneCode = countryPhoneCode;
    }

    public bool ArePropertiesNull(string[] propertyNames, out string? propertyName)
    {
        PropertyInfo[] properties = typeof(ApiUserModel).GetProperties();
        foreach(string pName in propertyNames)
        {
            if (!properties.Any(p => p.Name == pName))
                continue;

            PropertyInfo property = properties.Single(p => p.Name == pName);
            if(property.GetValue(this) is null)
            {
                propertyName = pName;
                return true;
            }
        }

        propertyName = null;
        return false;
    }

    public User ConvertToUser()
    {
        throw new NotImplementedException();
    }

}