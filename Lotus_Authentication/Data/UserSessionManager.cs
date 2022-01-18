using Blazored.LocalStorage;

namespace Lotus_Authentication.Data;

public class UserSessionManager
{

    /// <summary>
    /// An Action fired whenever the localstorage is updated. string parameter is the key that's updated.
    /// </summary>
    internal Func<string, Task> SessionHasChangedEvent;

    protected readonly ILocalStorageService LocalStorage;
    public UserSessionManager(ILocalStorageService localStorage)
    {
        LocalStorage = localStorage;
    }

    private void InvokeSessionChanged(string key)
    {
        if( SessionHasChangedEvent is not null)
        {
            SessionHasChangedEvent.Invoke(key);
        }
    }

    public async ValueTask Login()
    {
        await LocalStorage.SetItemAsync<bool>("loggedIn", true);
        InvokeSessionChanged("loggedIn");
    }

    public async ValueTask LogOut()
    {
        await LocalStorage.RemoveItemAsync("loggedIn");
        InvokeSessionChanged("loggedIn");
    }

    public async ValueTask AutoLogin()
    {
        if (await IsLoggedIn())
            await Login();
    }

    public async ValueTask<bool> IsLoggedIn()
    {
        bool isLoggedIn;
        try
        {
            isLoggedIn = await LocalStorage.GetItemAsync<bool>("loggedIn");
        }
        catch (Exception)
        {
            return false;
        }
        return isLoggedIn;
    }
}
