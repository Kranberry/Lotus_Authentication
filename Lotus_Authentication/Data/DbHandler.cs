using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;

namespace Lotus_Authentication.Data;

public class DbHandler
{
    private static IConfiguration _Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    private static string _ActiveDatabase = "authenticator";
    // _Configuration.GetConnectionString("authenticator");

    // GetUser overlaoded methods
    #region GetUser
    /// <summary>
    /// Get user from the databse
    /// </summary>
    /// <param name="userID">The unique id of the user</param>
    /// <returns>A new User object filled with data</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static User GetUser(int userID)
    {
        string sql = "SELECT TOP 1 * FROM [user] WHERE user_id = @id";
        using IDbConnection con = new SqlConnection(_Configuration.GetConnectionString(_ActiveDatabase));

        con.Open();
        dynamic? uD = con.Query<dynamic>(sql, new { id = userID }).FirstOrDefault();
        con.Close();

        if (uD is null)
            throw new UserNotFoundException(LogSeverity.Warning, $"User with id {userID} could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetUser)}(int userID)");

        User user = new(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, "SE", uD.record_insert_date, uD.record_update_date, uD.is_validated);
        return user;
    }

    /// <summary>
    /// Get user from the databse
    /// </summary>
    /// <param name="email">The unique email address of the user</param>
    /// <returns>A new User object filled with data</returns>
    public static User GetUser(string email)
    {
        string sql = "SELECT TOP 1 * FROM [user] WHERE email = @email";
        using IDbConnection con = new SqlConnection(_Configuration.GetConnectionString(_ActiveDatabase));

        con.Open();
        dynamic? uD = con.Query<dynamic>(sql, new { email = email }).FirstOrDefault();
        con.Close();

        if (uD is null)
            throw new UserNotFoundException(LogSeverity.Warning, $"User with email: {email} could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetUser)}(string email)");

        User user = new(uD.user_id, uD.first_name, uD.last_name, uD.email, uD.username, UserType.Regular, (Gender)uD.gender, GetCountryISO2ByID(uD.fk_country_id), uD.record_insert_date, uD.record_update_date, uD.is_validated);
        return user;
    }

    /// <summary>
    /// Get user from database by using either username or email and Sha1 encrypted password
    /// </summary>
    /// <param name="userName">The username for said user</param>
    /// <param name="email">The email addres of said user</param>
    /// <param name="password">The Sha1 encrypted password of said user</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static User GetUser(string? userName, string? email, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(email))
            throw new NotImplementedException();

        throw new NotImplementedException();
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
    /// Insert a new user into the database
    /// </summary>
    /// <param name="user">The user to be inserted</param>
    /// <returns>true if the operation was successful</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool InsertUser(User user)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Update user data in database
    /// </summary>
    /// <param name="user">The user object of said user</param>
    /// <returns>The updated user Object</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static User UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Permanently delete user from database
    /// </summary>
    /// <param name="id">The unique id of the user</param>
    /// <param name="email">The unique email of the user</param>
    /// <returns>true if the operation was successful</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool PermanentDeleteUser(int id, string email)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Permanently delete user from database
    /// </summary>
    /// <param name="id">The unique id of the user</param>
    /// <param name="email">The unique email of the user</param>
    /// <returns>true if the operation was successful</returns>
    public static bool RemoveUserApiConnection(int id, string email)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the country ISO2 code
    /// </summary>
    /// <param name="countryId">The primary key id of the country table</param>
    /// <returns></returns>
    public static string GetCountryISO2ByID(int countryId)
    {
        string sql = "SELECT TOP 1 * FROM [country] WHERE country_id = @id";
        using IDbConnection con = new SqlConnection(_Configuration.GetConnectionString(_ActiveDatabase));

        con.Open();
        dynamic? country = con.Query<dynamic>(sql, new { id = countryId }).FirstOrDefault();
        con.Close();

        if (country is null)
            throw new CountryNotFoundException(LogSeverity.Warning, $"Country with primary key {countryId} could not be found.", $"Class: {nameof(DbHandler)}, Method: {nameof(GetCountryISO2ByID)}(int countryId)");

        string iso2 = country.iso;
        return iso2;
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

        using IDbConnection db = new SqlConnection(_Configuration.GetConnectionString(_ActiveDatabase));
        
        db.Open();
        int affectedRowsCount = await db.ExecuteAsync(procedure
                                               , parameters
                                               , commandType: CommandType.StoredProcedure);
        db.Dispose();
        return affectedRowsCount;
    }
}