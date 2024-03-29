﻿using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lotus_Authentication.Data;

public class DbHandler
{
    // GetUser overlaoded methods
    #region GetUser
    /// <summary>
    /// Get user from the databse
    /// </summary>
    /// <param name="userID">The unique id of the user</param>
    /// <returns>A new User object filled with data</returns>
    /// <exception cref="UserNotFoundException">Thrown when user is not found</exception>
    public static User GetUser(int userID)
    {
        string sql = "SELECT TOP 1 * FROM [user] WHERE user_id = @id";
        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);

        con.Open();
        dynamic? uD = con.Query<dynamic>(sql, new { id = userID }).FirstOrDefault();
        con.Close();

        if (uD is null)
            throw new UserNotFoundException(LogSeverity.Warning, $"User with id {userID} could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetUser)}(int userID)");

        Country country = GetCountryByID(uD.fk_country_id);
        User user = new(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.is_validated);
        user.SetSalt(uD.salt);
        return user;
    }

    /// <summary>
    /// Get user from the databse
    /// </summary>
    /// <param name="email">The unique email address of the user</param>
    /// <param name="userType">The type of user to retrieve</param>
    /// <returns>A new User object filled with data</returns>
    /// <exception cref="UserNotFoundException">Thrown when user is not found</exception>
    public static User GetUser(string email, UserType userType = UserType.Regular)
    {
        string table = userType is UserType.Regular ? "user" : "api_user";
        string sql = $"SELECT TOP 1 * FROM [{table}] WHERE email = @email";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@email", email);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);

        con.Open();
        dynamic? uD = con.Query<dynamic>(sql, parameters).FirstOrDefault();
        con.Close();

        if (uD is null)
            throw new UserNotFoundException(LogSeverity.Warning, $"User with email: {email} could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetUser)}(string email)");

        Country country = GetCountryByID(uD.fk_country_id);
        User user;

        if (userType is UserType.Regular)
            user = new User(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.is_validated);
        else
            user = new User(uD.api_user_id, uD.contact_first_name, uD.contact_last_name, uD.email, UserType.Api, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.customer, uD.is_validated);
        
        user.SetSalt(uD.salt);
        return user;
    }

    /// <summary>
    /// Get user from database by using either username or email and Sha1 encrypted password
    /// </summary>
    /// <param name="userName">The username for said user</param>
    /// <param name="email">The email addres of said user</param>
    /// <param name="password">The Sha1 encrypted password of said user</param>
    /// <param name="userType">The type of user to retrieve</param>
    /// <param name="apiKey">The apiKey that tries to retrieve the user. Is used to check the ban status</param>
    /// <returns>The requested user</returns>
    /// <exception cref="UserNotFoundException">Thrown when user is not found</exception>
    public static User GetUser(string? userName, string? email, string password, UserType userType = UserType.Regular)
    {
        if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(email))
            throw new UserNotFoundException(LogSeverity.Warning, $"username and email cannot both be null.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetUser)}(string? userName, string? email, string password)");

        string table = userType is UserType.Regular ? "user" : "api_user";
        string sql = (userName, email) switch
        {
            (null or "",         not null or not "") => $"SELECT TOP 1 * FROM [{table}] WHERE email = @email AND password = @password",
            (not null or not "", null or "") => $"SELECT TOP 1 * FROM [{table}] WHERE username = @userName AND password = @password",
            (not null or not "", not null or not "") => $"SELECT TOP 1 * FROM [{table}] WHERE username = @userName AND email = @email AND password = @password"
        };

        DynamicParameters parameters = new();
        if (userName is not null)
            parameters.Add("@userName", userName, DbType.String);
        if (email is not null)
            parameters.Add("@email", email, DbType.String);

        byte[] storedSalt;
        if (email is null || !EmailValidator.IsValidEmail(email))
            storedSalt = GetUserByUserName(userName!).Salt!;
        else
            storedSalt = GetUser(email!, userType).Salt!;
        string hashedPass = SHA256Hash.HashString(password.ToUpper(), storedSalt);
        parameters.Add("@password", hashedPass, DbType.String);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);

        con.Open();
        dynamic? uD = con.Query<dynamic>(sql, parameters).FirstOrDefault();
        con.Close();

        if (uD is null)
            throw new UserNotFoundException(LogSeverity.Warning, $"User with email or username could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetUser)}(string? userName, string? email, string password)");

        Country country = GetCountryByID(uD.fk_country_id);

        if(userType is UserType.Regular)
        {
            User user = new(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.is_validated);
            return user;
        }

        return new User(uD.api_user_id, uD.contact_first_name, uD.contact_last_name, uD.email, UserType.Api, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.customer, uD.is_validated);
    }

    /// <summary>
    /// Get the banstatus of the user of said apiKey
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="apiKey">The apiKey to check for</param>
    /// <returns>A UserBanStatus object if the user has ever been banned, otherwise null, null means not banned</returns>
    public static UserBanStatus? GetUserBanStatus(User user, string apiKey)
    {
        UserBanStatus? userBanStatus = default;
        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        if (apiKey is not null)
        {
            string sqlQuery = $"SELECT * FROM [api_key_user_ban] akub INNER JOIN [api_key] ak ON ak.api_key_id = akub.fk_api_key_id WHERE akub.fk_user_id = { user.Id } AND ak.api_key = '{apiKey}'";
            con.Open();
            userBanStatus = con.Query<dynamic>(sqlQuery).Select(b => new UserBanStatus(user, apiKey, (BanStatus)b.record_status, b.ban_lift_date, b.reason)).FirstOrDefault();
            con.Close();
        }

        if(userBanStatus is not null)
            userBanStatus.BanStatus = (userBanStatus.BanLiftDate > DateTime.UtcNow) ? BanStatus.Banned : BanStatus.UnBanned;
        return userBanStatus;
    }

    /// <summary>
    /// Get user from the databse
    /// </summary>
    /// <param name="username">The unique username</param>
    /// <returns>A new User object filled with data</returns>
    /// <exception cref="UserNotFoundException">Thrown when user is not found</exception>
    private static User GetUserByUserName(string username)
    {
        string sql = "SELECT TOP 1 * FROM [user] WHERE username = @username";
        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);

        con.Open();
        dynamic? uD = con.Query<dynamic>(sql, new { username = username }).FirstOrDefault();
        con.Close();

        if (uD is null)
            throw new UserNotFoundException(LogSeverity.Warning, $"User with username {username} could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetUserByUserName)}(string username)");

        Country country = GetCountryByID(uD.fk_country_id);
        User user = new(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.is_validated);
        user.SetSalt(uD.salt);
        return user;
    }
    #endregion

    /// <summary>
    /// Get all users in database
    /// </summary>
    /// <returns>An IEnumerable containing every user in the database</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IEnumerable<User> GetUsers()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Get all users in database bound too said api key
    /// </summary>
    /// <param name="apiKey">The api key of the holder</param>
    /// <returns>An IEnumerable containing every user in the database bound to sent in api key</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IEnumerable<User> GetUsers(string apiKey)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if user has a connection to the requested API key
    /// </summary>
    /// <param name="userId">The unique id of the user</param>
    /// <param name="apiKeyId">The unique api key id of the api key</param>
    /// <returns>Returns true if the user is connected to the requested api key</returns>
    public static async ValueTask<bool> UserHasConnectionToApiKey(int userId, int apiKeyId)
    {
        if (userId <= 0 || apiKeyId <= 0)
            return false;

        string query = "SELECT TOP 1 user2api_key_id FROM [user2api_key] WHERE fk_user_id = @userId AND fk_api_key_id = @apiKeyId";
        DynamicParameters parameters = new();
        parameters.Add("@userId", userId, DbType.Int32);
        parameters.Add("@apiKeyId", apiKeyId, DbType.Int32);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();
        object objReturned = await con.ExecuteScalarAsync(query, parameters);
        con.Close();

        return objReturned is not null;
    }

    /// <summary>
    /// Insert a new user into the database. 
    /// </summary>
    /// <param name="user">The user to be inserted</param>
    /// <param name="apiKey">The apiKey to be used</param>
    /// <returns>true if the operation was successful</returns>
    /// <exception cref="UserAlreadyExistsException">This exception is thrown when there already exists a user with this email and/or username</exception>
    /// <exception cref="NullReferenceException">This exception is thrown when the password in the User object is null</exception>
    /// <exception cref="BadSHA1ReferenceException">This exception is thrown when the password in the User object is not a valid SHA1 checksum</exception>
    public static async Task<User> InsertUser(User user, string apiKey)
    {
        if (user.Password is null)
            throw new NullReferenceException("Password cannot be null " + nameof(user.Password));
        if (!SHA1Hash.IsValidSHA1(user.Password))
            throw new BadSHA1ReferenceException(LogSeverity.Informational, $"{apiKey} tried to insert user with invalid SHA1 checksum for password", $"Class: {nameof(DbHandler)}, Method: {nameof(InsertUser)}(User user, string? apiKey)");

        (string sha256Password, byte[] salt) = SHA256Hash.HashAndSaltString(user.Password);

        string procedure = user.UserType is UserType.Regular ? "[user_add]" : "[api_user_add]";
        DynamicParameters parameters = new();
        if(user.UserType is UserType.Regular)
        {
            parameters.Add("@username", user.UserName, DbType.String);
            parameters.Add("@api_key", apiKey, DbType.String);
        }
        else
        {
            parameters.Add("@customer", user.CompanyName, DbType.String);
        }

        parameters.Add(user.UserType is UserType.Regular ? "@first_name" : "@contact_first_name", user.FirstName, DbType.String);
        parameters.Add(user.UserType is UserType.Regular ? "@last_name" : "@contact_last_name", user.LastName, DbType.String);

        parameters.Add("@email", user.Email, DbType.String);
        parameters.Add("@password", sha256Password, DbType.String);
        parameters.Add("@salt", salt, DbType.Binary);
        parameters.Add("@country_iso2", user.CountryISO2, DbType.AnsiStringFixedLength, ParameterDirection.Input, 2);
        parameters.Add("@gender", user.Gender, DbType.Boolean);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();

        dynamic uD;
        try
        {
            uD = con.Query<dynamic>(procedure, parameters, commandType: CommandType.StoredProcedure).Single();
        }
        catch (SqlException ex)
        {
            if (ex.Number == (int)DatabaseException.UserAlreadyExists)
                throw new UserAlreadyExistsException(LogSeverity.Informational, $"User with username or email alreadt exists: userName{user.UserName}, email: {user.Email}", $"Class: {nameof(DbHandler)}, Method: {InsertUser}(User user, string ApiKey)");

            throw ex;
        }
        con.Close();

        Country country = GetCountryByID(uD.fk_country_id);
        User returnedUser = default;
        if(user.UserType is UserType.Regular)
            returnedUser = new(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.is_validated);
        else
            returnedUser = new(uD.api_user_id, uD.contact_first_name, uD.contact_last_name, uD.email, UserType.Api, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.customer, uD.is_validated);

        return returnedUser;
    }

    /// <summary>
    /// Update user data in database
    /// </summary>
    /// <param name="user">The user object of said user</param>
    /// <param name="updatePassword">The new password of the user (SHA1 hashed)</param>
    /// <returns>The updated user Object</returns>
    /// <exception cref="ArgumentIsNullException">Thrown when the Password property of the user object is null</exception>
    /// <exception cref="BadSHA1ReferenceException">Thrown when the Password property of the user object is not a valid SHA1 checksum</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user objects id is less than or equal to 0</exception>
    /// <exception cref="UserAlreadyExistsException">Thrown when the email or username already exists</exception>
    public static User UpdateUser(User user, bool updatePassword)
    {
        //TODO: Also make sure to be able to update api users with this. User user.UserType property
        if (updatePassword && string.IsNullOrWhiteSpace(user.Password))
            throw new ArgumentIsNullException(LogSeverity.Warning, $"{nameof(user.Password)} cannot be null or empty", $"Class: {nameof(DbHandler)}, Method: {UpdateUser}(User user, string ApiKey)");
        if (updatePassword && !SHA1Hash.IsValidSHA1(user.Password!))
            throw new BadSHA1ReferenceException(LogSeverity.Warning, "Password is not a valid SHA1 hashsum", $"Class: {nameof(DbHandler)}, Method: {UpdateUser}(User user, string ApiKey)");
        if (user.Id <= 0)
            throw new UserNotFoundException(LogSeverity.Warning, "User must exist for update", $"Class: {nameof(DbHandler)}, Method: {UpdateUser}(User user, string ApiKey)");

        if (updatePassword)
        {
            (string hash, byte[] salt) = SHA256Hash.HashAndSaltString(user.Password!);
            user.SetPassword(hash);
            user.SetSalt(salt);
        }

        //#error Do this next time. Procedure is created [user_update], but yet not tested
        string procedure = "[user_update]";
        DynamicParameters parameters = new();
        parameters.Add("@user_id", user.Id, DbType.Int32);
        parameters.Add("@username", user.UserName, DbType.String);
        parameters.Add("@email", user.Email, DbType.String);
        parameters.Add("@password", user.Password, DbType.String);
        parameters.Add("@salt", user.Salt, DbType.Binary);
        parameters.Add("@country_iso2", user.CountryISO2, DbType.AnsiStringFixedLength, ParameterDirection.Input, 2);
        parameters.Add("@first_name", user.FirstName, DbType.String);
        parameters.Add("@last_name", user.LastName, DbType.String);
        parameters.Add("@gender", (int)user.Gender, DbType.Int32);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();
        dynamic uD;
        try
        {
            uD = con.Query<dynamic>(procedure, parameters, commandType: CommandType.StoredProcedure).Single();
        }
        catch (SqlException ex)
        {
            BaseException exception = ex.Number switch
            {
                (int)DatabaseException.UserAlreadyExists => new UserAlreadyExistsException(LogSeverity.Informational, $"User with username or email already exists: userName - {user.UserName}, email: {user.Email}", $"Class: {nameof(DbHandler)}, Method: {UpdateUser}(User user, bool updatePassword)"),
                (int)DatabaseException.ParameterIsNull => new ArgumentIsNullException(LogSeverity.Informational, $"User id cannot be null when updating a user", $"Class: {nameof(DbHandler)}, Method: {UpdateUser}(User user, bool updatePassword)"),
                (int)DatabaseException.UserDoesNotExist => new UserNotFoundException(LogSeverity.Informational, $"User with this id does not exist: id - {user.Id}", $"Class: {nameof(DbHandler)}, Method: {UpdateUser}(User user, bool updatePassword)"),
                _ => throw ex
            };

            throw exception;
        }
        con.Close();

        Country country = GetCountryByID(uD.fk_country_id);
        User returnedUser = new(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, country.Iso2, country.NumCode, country.PhoneCode, uD.record_insert_date, uD.record_update_date, uD.is_validated);
        return returnedUser;
    }

    /// <summary>
    /// Permanently delete user from database
    /// </summary>
    /// <param name="id">The unique id of the user</param>
    /// <param name="email">The unique email of the user</param>
    /// <returns>true if the operation was successful</returns>
    public static bool PermanentDeleteUser(int id)
    {
        string procedure = "[permanently_delete_user]";
        DynamicParameters parameters = new();
        parameters.Add("@user_id", id);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();
        int rowsModified = con.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
        con.Close();

        return rowsModified > 0;
    }

    /// <summary>
    /// Modify the ban status of a user for your api key
    /// </summary>
    /// <param name="apiKey">Your api key</param>
    /// <param name="userId">The user to be banned</param>
    /// <param name="email">The users email to be banned (can be used instead of id)</param>
    /// <param name="banStatus">true/false, true for ban</param>
    /// <param name="wideBan">true/false, true if ban across all api keys on api users account</param>
    /// <param name="banLiftDate">Date when the ban will be lifted. Defaults to 1 day from now</param>
    /// <param name="reason">The reason to why user was banned</param>
    /// <returns>true if the users banstatus was modified successfully</returns>
    public static bool BanUser(string apiKey, int? userId, string? email, bool banStatus, bool wideBan, DateTime? banLiftDate, string? reason)
    {
        string procedure = "[modify_banstatus_of_user]";
        DynamicParameters parameters = new();
        parameters.Add("@api_key", apiKey);
        parameters.Add("@user_id", userId);
        parameters.Add("@email", email);
        parameters.Add("@ban_status", banStatus);
        parameters.Add("@wide_ban", wideBan);
        parameters.Add("@ban_lift_date", banLiftDate, DbType.DateTime);
        parameters.Add("@reason", reason);

        int rowsModified = 0;
        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();
        try
        {
            rowsModified = con.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        catch(SqlException ex)
        {
            return false;
        }
        con.Close();

        return rowsModified > 0;
    }

    /// <summary>
    /// Permanently delete a user to api connection
    /// </summary>
    /// <param name="id">The unique id of the user</param>
    /// <param name="apiKey">The unique apiKey of the api user</param>
    /// <returns>true if the operation was successful</returns>
    public static bool RemoveUserApiConnection(int id, string apiKey)
    {
        string procedure = "[delete_user2api_key]";
        ApiKey? apiKeyObj = GetApiKeyByApiKey(apiKey);
        if (apiKeyObj is null)
            return false;

        DynamicParameters parameters = new();
        parameters.Add("@user_id", id);
        parameters.Add("@api_key_id", apiKeyObj.ApiKeyID);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();
        int rowsModified = con.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
        con.Close();

        return rowsModified > 0;
    }

    /// <summary>
    /// Get the country ISO2 code
    /// </summary>
    /// <param name="countryId">The primary key id of the country table</param>
    /// <returns></returns>
    public static Country GetCountryByID(int countryId)
    {
        string sql = "SELECT TOP 1 * FROM [country] WHERE country_id = @id";
        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);

        con.Open();
        dynamic? countryDb = con.Query<dynamic>(sql, new { id = countryId }).FirstOrDefault();
        con.Close();

        if (countryDb is null)
            throw new CountryNotFoundException(LogSeverity.Warning, $"Country with primary key {countryId} could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetCountryByID)}(int countryId)");

        Country country = new(countryDb.country_id, countryDb.name, countryDb.nicename, countryDb.iso, countryDb.iso3, countryDb.numcode, countryDb.phonecode);
        return country;
    }

    /// <summary>
    /// Get every single country from the database
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Country> GetAllCountries()
    {
        string sql = "SELECT * FROM [country]";
        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);

        con.Open();
        IEnumerable<Country> countries = con.Query<dynamic>(sql).Select(c => new Country(c.country_id, c.name, c.nicename, c.iso, c.iso3, c.numcode, c.phonecode));
        con.Close();

        return countries;
    }

    /// <summary>
    /// Create a new api key connected to sent in api user
    /// </summary>
    /// <param name="apiUserId">The id of the api user the key belongs to. Must be a valid ID</param>
    /// <param name="alias">Text representation for the api key</param>
    /// <returns></returns>
    public static ApiKey InsertNewApiKey(int apiUserId, string alias = "")
    {
        if(alias.Length > 15)
            alias = alias[..15];

        if(apiUserId <= 0)
            throw new ArgumentOutOfRangeException(nameof(apiUserId));

        string procedure = "[create_api_key]";
        string generatedKey = ApiKey.GenerateApiKey();

        DynamicParameters parameters = new();
        parameters.Add("@api_user_id", apiUserId, DbType.Int32);
        parameters.Add("@api_key", generatedKey, DbType.String);
        parameters.Add("@alias", alias, DbType.String);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        
        con.Open();
        ApiKey apiKey = con.Query<dynamic>(procedure, parameters, commandType: CommandType.StoredProcedure)
                           .Select(a => new ApiKey() 
                                            { 
                                                Alias = a.alias
                                              , Key = a.api_key
                                              , ApiKeyID = a.api_key_id
                                              , InsertDate = a.record_insert_date
                                              , UpdateDate = a.record_update_date
                                              , RecordStatus = a.record_status
                                            }).Single();
        con.Close();

        return apiKey;
    }

    public static ApiKey ModifyStatusOfApiKey(ApiKey apiKey)
    {
        int newStatus = apiKey.RecordStatus == 0 ? 1 : 0;
        string query = $"UPDATE api_key SET " +
                       $"record_status = {newStatus}, " +
                       $"record_update_date = '{DateTime.UtcNow}' " +
                       $"WHERE api_key_id = {apiKey.ApiKeyID}";

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();
        int rowsAffected = con.Execute(query);
        con.Close();

        if(rowsAffected > 0)
        {
            apiKey.UpdateDate = DateTime.UtcNow;
            apiKey.RecordStatus = newStatus;
        }

        return apiKey;
    }

    /// <summary>
    /// Get every api key associated with api user
    /// </summary>
    /// <param name="userId">The id of the api user</param>
    /// <param name="apiUser">If the user is an api user or not</param>
    /// <returns></returns>
    public static IEnumerable<ApiKey> GetApiKeysByUserId(int userId, bool apiUser = true)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        (string table, string fkKey) userStuff = apiUser ? ("api_key2api_user", "fk_api_user_id") : ("user2api_key", "fk_user_id");
        string query =  "SELECT * FROM [api_key] AS a " +
                        $"INNER JOIN[{userStuff.table}] AS aia " +
                        "   ON aia.fk_api_key_id" +
                        " = a.api_key_id " +
                        $"WHERE aia.{userStuff.fkKey} = @userId";

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);

        DynamicParameters parameters = new();
        parameters.Add("@userId", userId, DbType.Int32);
        con.Open();
        IEnumerable<ApiKey> apiKeys = con.Query<dynamic>(query, parameters)
                           .Select(a => new ApiKey()
                           {
                               Alias = a.alias
                             , Key = a.api_key
                             , ApiKeyID = a.api_key_id
                             , InsertDate = a.record_insert_date
                             , UpdateDate = a.record_update_date
                             , RecordStatus = a.record_status
                           });

        return apiKeys;
    }

    /// <summary>
    /// Get the corresponding ApiKey object 
    /// </summary>
    /// <param name="apiKey"></param>
    /// <returns>An object of the api key found</returns>
    /// <exception cref="BadApiKeyReferenceException">Thrown when the api key could not be found</exception>
    public static ApiKey GetApiKeyByApiKey(string apiKey)
    {
        string query = "SELECT TOP 1 * FROM [api_key] WHERE [api_key] = @key";
        DynamicParameters parameters = new();
        parameters.Add("key", apiKey, DbType.String);

        using IDbConnection con = new SqlConnection(AppConfig.ActiveDatabaseCS);
        con.Open();
        ApiKey? apiKeyObj = con.Query<dynamic>(query, parameters).Select(item => new ApiKey() 
                                                                        {
                                                                            Alias = item.alias,
                                                                            ApiKeyID = item.api_key_id,
                                                                            InsertDate = item.record_insert_date,
                                                                            UpdateDate = item.record_update_date,
                                                                            Key = item.api_key
                                                                        }).SingleOrDefault();
        con.Close();
        if (apiKeyObj is null)
            throw new BadApiKeyReferenceException(LogSeverity.Warning, $"Api key not found {apiKey}", $"Class: {nameof(DbHandler)}, Method: {nameof(GetApiKeyByApiKey)}(string apiKey)");

        return apiKeyObj;
    }

    /// <summary>
    /// Add a new system log to the database
    /// </summary>
    /// <param name="severity">How severe this is</param>
    /// <param name="exception">The exception thrown (if any)</param>
    /// <param name="message">An informational message about the log</param>
    /// <param name="page">What class and method. Where did this happen</param>
    /// <returns>An int referencing the amount of rows affected</returns>
    public static async ValueTask<int> AddNewSystemLog(LogSeverity severity, Exception? exception, string message, string page)
    {
        string procedure = "[system_logs_add]";
        DynamicParameters parameters = new();
        parameters.Add("@application", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, DbType.String);
        parameters.Add("@severity", severity.ToString(), DbType.String);
        parameters.Add("@exception_type", exception?.GetType().Name, DbType.String);
        parameters.Add("@message", message, DbType.String);
        parameters.Add("@page", page, DbType.String);
        parameters.Add("@stacktrace", exception?.StackTrace, DbType.String);

        using IDbConnection db = new SqlConnection(AppConfig.ActiveDatabaseCS);
        
        db.Open();
        int affectedRowsCount = await db.ExecuteAsync(procedure
                                               , parameters
                                               , commandType: CommandType.StoredProcedure);
        db.Dispose();
        return affectedRowsCount;
    }
}