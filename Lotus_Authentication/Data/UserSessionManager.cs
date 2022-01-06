using Blazored.LocalStorage;

namespace Lotus_Authentication.Data;

public class UserSessionManager
{
    protected readonly ILocalStorageService LocalStorage;

    public UserSessionManager(ILocalStorageService localStorage)
    {
        LocalStorage = localStorage;
    }

    public async ValueTask Login()
    {
        await LocalStorage.SetItemAsync<bool>("loggedIn", true);
    }

    public async ValueTask LogOut()
    {
        await LocalStorage.RemoveItemAsync("loggedIn");
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
