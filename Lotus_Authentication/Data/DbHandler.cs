using Dapper;
using System.Data.SqlClient;

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
    /// <exception cref="NotImplementedException"></exception>
    public static User GetUser(int userID)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get user from the databse
    /// </summary>
    /// <param name="email">The unique email address of the user</param>
    /// <returns>A new User object filled with data</returns>
    public static User GetUser(string email)
    {
        throw new NotImplementedException();
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
    /// Add a new system log to the database
    /// </summary>
    /// <param name="severity">How severe this is</param>
    /// <param name="exception">The exception thrown (if any)</param>
    /// <param name="message">An informational message about the log</param>
    /// <param name="page">What class and method. Where did this happen</param>
    public static void AddNewSystemLog(LogSeverity severity, Exception? exception, string message, string page)
    {
        /* [exception_type] is taken from the exception parameter
         * [stacktrace] is taken from the exception parameter
         * [application] is the name taken from this application
         * [severity] will be the enum converted to a string
         */
        throw new NotImplementedException();
    }
}