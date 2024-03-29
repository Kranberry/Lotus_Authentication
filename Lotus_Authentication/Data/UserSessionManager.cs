﻿using Blazored.LocalStorage;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using System.Text.Json;

namespace Lotus_Authentication.Data;

internal enum SessionState
{
    LoggedIn,
    LoggedOut,
    TokenExpired,
    TokenInvalid
}

public class UserSessionManager
{

    /// <summary>
    /// An Func fired whenever the localstorage is updated. string parameter is the key that's updated.
    /// </summary>
    internal Func<SessionState, Task> SessionHasChangedEvent;

    protected readonly ILocalStorageService LocalStorage;
    public UserSessionManager(ILocalStorageService localStorage)
    {
        LocalStorage = localStorage;
    }

    private void InvokeSessionChanged(SessionState key)
    {
        if( SessionHasChangedEvent is not null)
        {
            SessionHasChangedEvent.Invoke(key);
        }
    }

    /// <summary>
    /// Login the user and add a jwt token to represent it
    /// </summary>
    /// <param name="user">The user to represent</param>
    /// <returns></returns>
    public async ValueTask Login(User user)
    {
        if (user.Password is null || !SHA1Hash.IsValidSHA1(user.Password))
            throw new BadSHA1ReferenceException(LogSeverity.Warning, "Could not authenticate user with invalid password format", $"Class: {nameof(UserSessionManager)}, Method: {nameof(Login)}(User user)");
        else if (string.IsNullOrWhiteSpace(user.Email) && string.IsNullOrWhiteSpace(user.UserName))
            throw new ArgumentException("username and email cannot be null or empty");

        string token = JwtBuilder.Create()
                                 .WithAlgorithm(new HMACSHA256Algorithm())
                                 .WithSecret(AppConfig.JWTSymmetricSecret)
                                 .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds())
                                 .AddClaim("username", user.UserName)
                                 .AddClaim("email", user.Email)
                                 .AddClaim("password", user.Password)
                                 .AddClaim("userType", user.UserType.ToString())
                                 .Encode();
        await LocalStorage.SetItemAsStringAsync("jwt", token);

        InvokeSessionChanged(SessionState.LoggedIn);
    }

    public async ValueTask LogOut()
    {
        await LocalStorage.RemoveItemAsync("jwt");
        InvokeSessionChanged(SessionState.LoggedOut);
    }

    public async ValueTask<bool> IsLoggedIn(bool isApiUser = false, bool anyUser = true)
    {
        string jwt;
        try
        {
            jwt = await LocalStorage.GetItemAsync<string>("jwt");
            IDictionary<string, object> payload = JwtBuilder.Create()
                                                            .WithAlgorithm(new HMACSHA256Algorithm())
                                                            .WithSecret(AppConfig.JWTSymmetricSecret)
                                                            .MustVerifySignature()
                                                            .Decode<IDictionary<string, object>>(jwt);

            UserType userType = (UserType)Enum.Parse(typeof(UserType), (string)payload["userType"]);
            if (isApiUser && userType is not UserType.Api && !anyUser)
                return false;
            if (!isApiUser && userType is UserType.Api && !anyUser)
                return false;
        }
        catch (TokenExpiredException)
        {
            InvokeSessionChanged(SessionState.TokenExpired);
            return false;
        }
        catch (SignatureVerificationException)
        {
            InvokeSessionChanged(SessionState.TokenInvalid);
            return false;
        }
        catch(Exception e)
        {
            return false;
        }
        return true;
    }

    internal async Task<User> GetCurrentUser(bool isApiUser = false, bool anyUser = true)
    {
        if (!(await IsLoggedIn(isApiUser, anyUser)))
            throw new UserNotFoundException(LogSeverity.Warning, "No user is found logged in", $"Class: {nameof(UserSessionManager)}, Method: {nameof(GetCurrentUser)}()");

        string jwt = await LocalStorage.GetItemAsync<string>("jwt");
        IDictionary<string, object> payload = JwtBuilder.Create()
                                                        .WithAlgorithm(new HMACSHA256Algorithm())
                                                        .WithSecret(AppConfig.JWTSymmetricSecret)
                                                        .MustVerifySignature()
                                                        .Decode<IDictionary<string, object>>(jwt);

        UserType userType = (UserType)Enum.Parse(typeof(UserType), (string)payload["userType"]);
        return DbHandler.GetUser((string)payload["email"], userType);
    }
}
