using Microsoft.AspNetCore.Components.Web;
using System.Text.RegularExpressions;

namespace Lotus_Authentication.Pages;

public partial class ApiUserManagement : IDisposable
{
    private string LoginEmail { get; set; } = "";
    private string LoginPassword { get; set; } = "";
    private User? CurrentUser { get; set; }
    private List<ApiKey> UserApiKeys { get; set; } = new List<ApiKey>();
    private List<ApiKey> modifyApiKeys { get; set; } = new List<ApiKey>();
    private string NewApeKeyAlias = "";

    private bool DoRegister = false;

    private bool SweetAlertHidden { get; set; } = true;
    private string SweetAlertText { get; set; } = "We have sent you an email";
    private SweetAlertIcons SweetAlertIcon { get; set; } = SweetAlertIcons.Success;

    private Gender SelectedGender = Gender.Other;
    private string[] GenderOptions = Enum.GetNames(typeof(Gender));

    private IEnumerable<Country> AvailableCountries = DbHandler.GetAllCountries();
    private string[] CountryOptions = Array.Empty<string>();
    private Country SelectedCountry { get; set; }

    private class InputValidThing
    {
        internal string Value { get; set; } = "";
        internal bool IsValid { get; set; } = false;
    }

    private Dictionary<string, InputValidThing> InputFields = new()
    {
        { "ContactFirstName", new() },
        { "ContactLastName", new() },
        { "EmailInput", new() },
        { "CompanyName", new() },
        { "Password", new() },
        { "PasswordAgain", new() },
        { "Gender", new() },
        { "Country", new() }
    };

    protected override async Task OnParametersSetAsync()
    {
        if (await Session.IsLoggedIn(isApiUser: false, anyUser: false))
        {
            NavManager.NavigateTo("/");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            modifyApiKeys = new();
            UserApiKeys = new();
            CountryOptions = AvailableCountries.Select(c => c.NiceName).ToArray();
            await SetCurrentuser(SessionState.LoggedIn);
            Session.SessionHasChangedEvent += SetCurrentuser;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task SetCurrentuser(SessionState state)
    {
        try
        {
            if (state == SessionState.LoggedIn)
            {
                CurrentUser = await Session.GetCurrentUser(true, false);
                FetchApiKeys();
            }
            else
                CurrentUser = null;
        }
        catch (UserNotFoundException)
        {
            CurrentUser = null;
        }
    }

    private bool IsPasswordValid(string password)
    {
        Regex atleastOneNumber = new(@"\d");
        Regex atleastOneLowerCase = new(@"[A-Z]");
        Regex atleastOneUpperCase = new(@"[a-z]");

        return atleastOneLowerCase.IsMatch(password) && atleastOneUpperCase.IsMatch(password) && atleastOneNumber.IsMatch(password) && password.Length >= 8;
    }

    private void ToggleRegister() => DoRegister = !DoRegister;

    private void ShowSweetAlert(string text, SweetAlertIcons icon)
    {
        SweetAlertIcon = icon;
        SweetAlertText = text;
        SweetAlertHidden = false;
    }

    private async Task TryRegisterApiUser()
    {
        InputFields["EmailInput"].IsValid = EmailValidator.IsValidEmail(InputFields["EmailInput"].Value);
        InputFields["ContactFirstName"].IsValid = InputFields["ContactFirstName"].Value.Length >= 1;
        InputFields["ContactLastName"].IsValid = InputFields["ContactLastName"].Value.Length >= 1;
        InputFields["CompanyName"].IsValid = InputFields["CompanyName"].Value.Length >= 1;
        InputFields["Password"].IsValid = IsPasswordValid(InputFields["Password"].Value);
        InputFields["PasswordAgain"].IsValid = InputFields["Password"].Value == InputFields["PasswordAgain"].Value;
        InputFields["Gender"].IsValid = InputFields["Gender"] is not null;
        InputFields["Country"].IsValid = InputFields["Country"] is not null;

        bool allFieldsValid = InputFields.Where(kvp => !kvp.Value.IsValid).Count() == 0;
        if (SelectedCountry is null)
            SelectedCountry = AvailableCountries.Single(c => c.Id == 1);

        if (allFieldsValid)
        {
            User user = new(0, InputFields["ContactFirstName"].Value, InputFields["ContactLastName"].Value, InputFields["EmailInput"].Value, UserType.Api, SelectedGender, SelectedCountry.Iso2, SelectedCountry.NumCode, SelectedCountry.PhoneCode, DateTime.UtcNow, null, InputFields["CompanyName"].Value, false);
            user.SetPassword(SHA1Hash.Hash(InputFields["Password"].Value));
            try
            {
                await DbHandler.InsertUser(user, AppConfig.SkeletonKey);

                foreach (KeyValuePair<string, InputValidThing> kvp in InputFields)
                    kvp.Value.Value = "";

                ShowSweetAlert("We have sent you and email", SweetAlertIcons.Success);
            }
            catch (UserAlreadyExistsException)
            {
                ShowSweetAlert("This email is already taken", SweetAlertIcons.Danger);
            }
        }
        else
        {
            ShowSweetAlert("All fields are mandatory", SweetAlertIcons.Danger);
        }
    }

    private async Task LoginApiUser()
    {
        try
        {
            string sha1Pass = SHA1Hash.Hash(LoginPassword);
            CurrentUser = DbHandler.GetUser(null, LoginEmail, sha1Pass, UserType.Api);
            CurrentUser.SetPassword(sha1Pass);
            await Session.Login(CurrentUser);
        }
        catch (UserNotFoundException)
        {
            ShowSweetAlert("Invalid email or password", SweetAlertIcons.Danger);
        }
        catch (ArgumentNullException)
        {
            ShowSweetAlert("Invalid email or password", SweetAlertIcons.Danger);
        }
    }

    private void FetchApiKeys()
    {
        UserApiKeys = DbHandler.GetApiKeysByUserId(CurrentUser.Id).ToList();
        InvokeAsync(StateHasChanged);
    }

    private void GenerateNewApiKey()
    {
        UserApiKeys.Add(DbHandler.InsertNewApiKey(CurrentUser!.Id, NewApeKeyAlias));
    }

    private void AddModifiedApiKey(int apiKeyId, bool willAdd)
    {
        if(willAdd)
        {
            modifyApiKeys.Add(UserApiKeys.Single(a => a.ApiKeyID == apiKeyId));
        }
        else
        {
            modifyApiKeys.RemoveAll(a => a.ApiKeyID == apiKeyId);
        }
        InvokeAsync(StateHasChanged);
    }

    private void ModifyStatusOfApiKeys()
    {
        foreach(ApiKey key in modifyApiKeys)
        {
            DbHandler.ModifyStatusOfApiKey(key);
        }
        InvokeAsync(StateHasChanged);
        modifyApiKeys.Clear();
    }

    private void OnLoginKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Key == "NumpadEnter")
        {

        }
    }

    public void Dispose()
    {
        Session.SessionHasChangedEvent -= SetCurrentuser;
    }
}
